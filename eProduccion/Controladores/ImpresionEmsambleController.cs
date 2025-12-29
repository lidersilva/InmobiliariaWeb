using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.Common;
using iTextSharp.text;
using PdfiumViewer;
using PdfiumPdfDocument = PdfiumViewer.PdfDocument;     



namespace eProduccion.Controladores
{
    public class ImpresionEmsambleController
    {
        public class TicketDataEmsamble
        {
            public string Operador { get; set; }
            public DateTime? FechaRecepcion { get; set; }
            public DateTime? HoraRecepcion { get; set; }            
            public string Producto { get; set; }
            public DateTime? FechaFin { get; set; }
            public DateTime? HoraFin { get; set; }
            public double CantidadEntregado { get; set; }
            public string PerfilAcabado { get; set; } = "ACABADO ESTÁNDAR";
        }

        public string GenerarHtmlTicket(TicketDataEmsamble t)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "TicketEmsamble.html");
            // Leer en UTF-8 para que los acentos no se rompan
            string html = File.ReadAllText(templatePath, Encoding.UTF8);

            // Barcode basado en Producto
            string ean = EnsureEan13(t.Producto ?? string.Empty);
            string barcodeFile = SaveBarcodePng(ean);
            string filePathForHtml = barcodeFile?.Replace("\\", "/");
            string imgTag = !string.IsNullOrEmpty(filePathForHtml)
                ? $"<img src=\"{filePathForHtml}\" class=\"barcode\" style=\"display:block;margin:2px auto;max-width:60mm;height:auto;max-height:12mm;\" />"
                : string.Empty;

            // Valores
            string operador = t.Operador ?? string.Empty;
            string fechaRecepcion = (t.FechaRecepcion ?? DateTime.Now).ToString("dd/MM/yyyy");
            string horaRecepcion = (t.HoraRecepcion ?? DateTime.Now).ToString("HH:mm");
            string producto = t.Producto ?? string.Empty;
            string fechaFin = (t.FechaFin ?? DateTime.Now).ToString("dd/MM/yyyy");
            string horaFin = (t.HoraFin ?? DateTime.Now).ToString("HH:mm");
            string cantidad = t.CantidadEntregado.ToString();

            // Reemplazos estilo Inyección
            html = html.Replace("{{BARCODE_IMAGE}}", imgTag)
                       .Replace("{{OPERADOR}}", operador)
                       .Replace("{{FECHA_RECEPCION}}", fechaRecepcion)
                       .Replace("{{HORA_RECEPCION}}", horaRecepcion)
                       .Replace("{{PRODUCTO}}", producto)
                       .Replace("{{FECHA_FIN}}", fechaFin)
                       .Replace("{{HORA_FIN}}", horaFin)
                       .Replace("{{CANTIDAD}}", cantidad);

            if (html.Contains("{{PERFIL}}"))
                html = html.Replace("{{PERFIL}}", t.PerfilAcabado ?? "ACABADO ESTÁNDAR");

            return html;
        }

        public string CrearPdfDesdeHtml_iTextSharp(string html)
        {
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            string outputPath = Path.Combine(pdfFolder, $"ticket_{Guid.NewGuid()}.pdf");

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Document document = new Document(PageSize.A6, 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                using (var sr = new StringReader(html))
                {
                    HTMLWorker htmlWorker = new HTMLWorker(document);
                    htmlWorker.Parse(sr);
                }

                document.Close();
            }

            return outputPath;
        }

        private float MmToPoints(float mm)
        {
            return mm / 25.4f * 72f;
        }

        // Reemplaza el método CrearPdfDesdeHtml_ConCss por esta versión que fuerza tamaño exacto (75 x 25 mm),
        // inyecta CSS compacto para evitar salto a segunda página y asegura que el barcode que se guardó como PNG
        // tenga un tamaño máximo razonable para entrar en una sola hoja.
        public string CrearPdfDesdeHtml_ConCss(string html, float widthMm = 75f, float heightMm = 45f)
        {
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            string outputPath = Path.Combine(pdfFolder, $"ticket_{Guid.NewGuid()}.pdf");

            // Recoger rutas de PNG temporales (barcode_*.png) generadas por GenerarHtmlTicket
            var tempImagePaths = new List<string>();
            foreach (Match m in Regex.Matches(html ?? string.Empty, @"(?:[A-Za-z]:\\|/)[^\s'\""]*barcode_[A-Za-l0-9\-]+\.png", RegexOptions.IgnoreCase))
            {
                tempImagePaths.Add(m.Value.Replace("/", Path.DirectorySeparatorChar.ToString()));
            }

            try
            {
                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    float w = MmToPoints(widthMm);
                    float h = MmToPoints(heightMm);

                    var pageSize = new iTextSharp.text.Rectangle(w, h);
                    var document = new Document(pageSize, 0, 0, 0, 0);
                    var writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    // CSS compacto: fuente pequeña, paddings mínimos y limitar la altura del barcode (evitar salto)
                    var compactCss = @"<style>
                @page { size: " + widthMm + "mm " + heightMm + @"mm; margin: 0; }
                html, body { margin:0; padding:0; font-family: Arial, Helvetica, sans-serif; font-size:7px; color:#000; }
                table { width:100%; border-collapse:collapse; table-layout:fixed; font-size:7px; }
                td { padding:1px 3px; vertical-align:middle; }
                img.barcode { display:block; margin:2px auto; max-width:60mm; max-height:12mm; height:auto; }
                </style>";

                    // Quitar data:URIs por seguridad y anteponer CSS
                    string htmlSafe = Regex.Replace(html ?? string.Empty, @"<img[^>]*src=['""]data:[^'""]+['""][^>]*>", "", RegexOptions.IgnoreCase);
                    string htmlWithCss = compactCss + htmlSafe;
                    string xhtml = NormalizeHtmlToXhtml(htmlWithCss);

                    // Parsear HTML con XMLWorker y permitir que procese la imagen grande directamente
                    try
                    {
                        using (var sr = new StringReader(xhtml))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"XMLWorker error: {ex.Message}");
                        // Fallback simple (HTMLWorker)
                        try
                        {
                            using (var sr = new StringReader(htmlWithCss))
                            {
                                var hw = new HTMLWorker(document);
                                hw.Parse(sr);
                            }
                        }
                        catch (Exception inner)
                        {
                            Console.WriteLine($"HTMLWorker fallback error: {inner.Message}");
                        }
                    }

                    document.Close();
                }

                return outputPath;
            }
            finally
            {
                // Limpieza: eliminar PNG temporales generados para el barcode
                foreach (var path in tempImagePaths)
                {
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    catch
                    {
                        // no interrumpir por error de cleanup
                    }
                }
            }
        }


        public void PrintPdf(string pdfPath, string printerName = null, bool abrirEnVisor = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pdfPath))
                    throw new ArgumentException("La ruta del PDF no puede estar vacía.");

                if (!File.Exists(pdfPath))
                    throw new FileNotFoundException($"No se encontró el archivo PDF en la ruta: {pdfPath}");

                if (abrirEnVisor)
                {
                    OpenPdfWithDefaultApp(pdfPath);
                    return;
                }

                // Cargar documento PDF usando PdfiumViewer (requiere System.Windows.Forms disponible)
                using (var document = PdfiumPdfDocument.Load(pdfPath))
                using (var printDocument = document.CreatePrintDocument())
                {
                    var printerSettings = new PrinterSettings();

                    // Si no se pasa impresora, usa la predeterminada del sistema
                    if (!string.IsNullOrWhiteSpace(printerName))
                    {
                        try
                        {
                            printerSettings.PrinterName = printerName;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Nombre de impresora inválido '{printerName}': {ex.Message}");
                        }
                    }

                    printDocument.PrinterSettings = printerSettings;
                    printDocument.PrintController = new StandardPrintController(); // evita diálogo
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al imprimir/abrir PDF: {ex.Message}");
            }
        }

        private void OpenPdfWithDefaultApp(string pdfPath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", pdfPath);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", pdfPath);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo abrir el visor de PDF: {ex.Message}");
            }
        }

        private string NormalizeHtmlToXhtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            // Cerrar meta tag HTML5 (<meta charset="..."> -> <meta charset="..." />)
            html = Regex.Replace(html, @"<meta\b([^>]*)>", m =>
            {
                var s = m.Value;
                if (s.EndsWith("/>")) return s;
                return s.TrimEnd('>') + " />";
            }, RegexOptions.IgnoreCase);

            // Cerrar img tags (<img ...> -> <img ... />)
            html = Regex.Replace(html, @"<img\b([^>]*)>", m =>
            {
                var s = "<img" + m.Groups[1].Value;
                if (s.EndsWith("/")) return s + ">";
                if (s.EndsWith("/>")) return s + ">";
                return s + " />";
            }, RegexOptions.IgnoreCase);

            // Cerrar other void elements common
            string[] voids = { "br", "hr", "link", "input", "col", "base", "embed", "source", "track", "param" };
            foreach (var tag in voids)
            {
                html = Regex.Replace(html, $@"<{tag}\b([^>]*)>", m =>
                {
                    var s = m.Value;
                    if (s.EndsWith("/>")) return s;
                    return s.TrimEnd('>') + " />";
                }, RegexOptions.IgnoreCase);
            }

            // Opcional: forzar encoding meta well-formed (si usas meta http-equiv)
            html = Regex.Replace(html, @"<meta([^>]*http-equiv[^>]*)charset=([^>]*?)>", m => m.Value, RegexOptions.IgnoreCase);

            return html;
        }

        // Método de prueba que ya funcionó localmente: genera solo el barcode en un PDF
        public string CrearPdfSoloBarcode(string codigoArticulo, float widthMm = 75f, float heightMm = 25f)
        {
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            string outputPath = Path.Combine(pdfFolder, $"barcode_test_{Guid.NewGuid()}.pdf");

            try
            {
                string ean = EnsureEan13(codigoArticulo ?? string.Empty);
                string base64 = GenerateEan13BarcodeBase64(ean);

                if (string.IsNullOrWhiteSpace(base64))
                {
                    Console.WriteLine("No se generó base64 del código de barras.");
                    return outputPath;
                }

                byte[] imgBytes = Convert.FromBase64String(base64);

                float w = MmToPoints(widthMm);
                float h = MmToPoints(heightMm);

                var pageSize = new iTextSharp.text.Rectangle(w, h);

                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var document = new Document(pageSize, 0, 0, 0, 0);
                    var writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    var pdfImage = iTextSharp.text.Image.GetInstance(imgBytes);

                    float maxWidth = w - 6f;
                    float maxHeight = h - 6f;
                    pdfImage.ScaleToFit(maxWidth, maxHeight);

                    float posX = (w - pdfImage.ScaledWidth) / 2f;
                    float posY = (h - pdfImage.ScaledHeight) / 2f;
                    pdfImage.SetAbsolutePosition(posX, posY);

                    writer.DirectContent.AddImage(pdfImage);

                    document.Close();
                }

                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando PDF solo barcode: {ex.Message}");
                return string.Empty;
            }
        }

        private string SaveBarcodePng(string ean13)
        {
            try
            {
                // Generar base64 usando el helper existente
                string base64 = GenerateEan13BarcodeBase64(ean13);
                if (string.IsNullOrWhiteSpace(base64))
                    return null;

                byte[] bytes = Convert.FromBase64String(base64);

                // Guardar en wwwroot/pdf (carpeta ya usada por el servicio)
                string outDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                string fileName = $"barcode_{Guid.NewGuid():N}.png";
                string fullPath = Path.Combine(outDir, fileName);

                File.WriteAllBytes(fullPath, bytes);

                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando PNG del barcode: {ex.Message}");
                return null;
            }
        }

        private string EnsureEan13(string input)
        {
            var digits = new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length >= 12)
            {
                digits = digits.Substring(0, 12);
            }
            else
            {
                digits = digits.PadLeft(12, '0');
            }

            int checksum = CalculateEan13Checksum(digits);
            return digits + checksum.ToString();
        }

        private int CalculateEan13Checksum(string twelveDigits)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int d = twelveDigits[i] - '0';
                if ((i % 2) == 1)
                    sum += d * 3;
                else
                    sum += d;
            }

            int mod = sum % 10;
            int check = (10 - mod) % 10;
            return check;
        }

        private string GenerateEan13BarcodeBase64(string ean13)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.EAN_13,
                    Options = new EncodingOptions
                    {
                        Height = 60,
                        Width = 300,
                        Margin = 2,
                        PureBarcode = false
                    }
                };

                var pixelData = writer.Write(ean13);

                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating barcode: {ex.Message}");
                return string.Empty;
            }
        }
    }
}

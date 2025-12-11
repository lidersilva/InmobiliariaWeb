using DinkToPdf;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Path = System.IO.Path;
using PdfDocument = PdfiumViewer.PdfDocument;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using iTextSharp.tool.xml;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace eProduccion.Data
{
    public class TicketData
    {
        public int NumeroOT { get; set; }
        public string NumeroCaja { get; set; }
        public string CodigoArticulo { get; set; }
        public int CantidadAprobadas { get; set; }
        public string Maquina { get; set; }
        public string Operador { get; set; }
        public DateTime Fecha { get; set; }
        public string Turno { get; set; }
        public string PerfilAcabado { get; set; } = "ACABADO ESTÁNDAR";
    }

    public class ImpresionEtiquetaService
    {
        // -----------------------
        // Helpers MOVIDOS ARRIBA
        // -----------------------
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

        // -----------------------
        // API pública
        // -----------------------
        public async Task ImprimirEtiquetaInyeccionExtrusion(bool imprimirNoConformes, int nroOT, string nroCaja, string codArticulo, int cantAprobadas, int cantRetenidas, int cantRechReciclable,
            int cantRechNoReciclable, string maquina, string operador, DateTime? fecha, string turno)
        {
            var ticket = new TicketData
            {
                NumeroOT = nroOT,
                NumeroCaja = nroCaja,
                CodigoArticulo = codArticulo,
                CantidadAprobadas = cantAprobadas,
                Maquina = maquina,
                Operador = operador,
                Fecha = fecha ?? DateTime.Now,
                Turno = turno
            };

            string html = GenerarHtmlTicket(ticket);
            string pdfPath = CrearPdfDesdeHtml_ConCss(html);
            PrintPdf(pdfPath);
        }

        //public string GenerarHtmlTicket(TicketData t)
        //{
        //    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ticket.html");
        //    string html = File.ReadAllText(templatePath);

        //    // Generar barcode EAN-13 a partir de CodigoArticulo y guardarlo como PNG temporal
        //    string ean = EnsureEan13(t.CodigoArticulo ?? string.Empty);
        //    string barcodeFile = SaveBarcodePng(ean); // ruta absoluta al PNG

        //    // Usar file URI para que XMLWorker cargue el PNG desde disco
        //    string fileUri = barcodeFile != null ? new Uri(barcodeFile).AbsoluteUri : string.Empty;
        //    string imgTag = !string.IsNullOrEmpty(fileUri)
        //        ? $"<img src=\"{fileUri}\" style=\"display:block;margin:4px auto;max-width:60mm;height:auto;\" />"
        //        : string.Empty;

        //    html = html.Replace("{{BARCODE_IMAGE}}", imgTag)
        //               .Replace("{{NRO_OP}}", t.NumeroOT.ToString())
        //               .Replace("{{NRO_CAJA}}", t.NumeroCaja)
        //               .Replace("{{PRODUCTO}}", t.CodigoArticulo)
        //               .Replace("{{CANTIDAD}}", t.CantidadAprobadas.ToString())
        //               .Replace("{{MAQUINA}}", t.Maquina)
        //               .Replace("{{OPERADOR}}", t.Operador)
        //               .Replace("{{FECHA}}", t.Fecha.ToString("dd/MM/yyyy"))
        //               .Replace("{{TURNO}}", t.Turno)
        //               .Replace("{{PERFIL}}", t.PerfilAcabado);

        //    return html;
        //}
        public string GenerarHtmlTicket(TicketData t)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ticket.html");
            string html = File.ReadAllText(templatePath);

            // Generar barcode EAN-13 y guardar como PNG temporal
            string ean = EnsureEan13(t.CodigoArticulo ?? string.Empty);
            string barcodeFile = SaveBarcodePng(ean); // ruta absoluta al PNG

            // XMLWorker NO acepta "file:///" → usar ruta física normal
            string fileUri = barcodeFile?.Replace("\\", "/");

            // Etiqueta <img> corregida
            string imgTag = !string.IsNullOrEmpty(fileUri)
                ? $"<img src=\"{fileUri}\" style=\"display:block;margin:4px auto;max-width:60mm;height:auto;\" />"
                : string.Empty;

            html = html.Replace("{{BARCODE_IMAGE}}", imgTag)
                       .Replace("{{NRO_OP}}", t.NumeroOT.ToString())
                       .Replace("{{NRO_CAJA}}", t.NumeroCaja)
                       .Replace("{{PRODUCTO}}", t.CodigoArticulo)
                       .Replace("{{CANTIDAD}}", t.CantidadAprobadas.ToString())
                       .Replace("{{MAQUINA}}", t.Maquina)
                       .Replace("{{OPERADOR}}", t.Operador)
                       .Replace("{{FECHA}}", t.Fecha.ToString("dd/MM/yyyy"))
                       .Replace("{{TURNO}}", t.Turno)
                       .Replace("{{PERFIL}}", t.PerfilAcabado);

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

        public string CrearPdfDesdeHtml_ConCssCm(string html, float widthCm = 7.5f, float heightCm = 2.5f)
        {
            return CrearPdfDesdeHtml_ConCss(html, widthCm * 10f, heightCm * 10f);
        }

        // Versión que parsea el HTML y permite que XMLWorker cargue la imagen desde archivo
        public string CrearPdfDesdeHtml_ConCss(string html, float widthMm = 75f, float heightMm = 25f)
        {
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            string outputPath = Path.Combine(pdfFolder, $"ticket_{Guid.NewGuid()}.pdf");

            // Recoger rutas de imágenes temporales para eliminar después
            var tempImageUris = new List<string>();

            try
            {
                // Detectar file:///...barcode_*.png incrustados en el HTML (generados por GenerarHtmlTicket)
                foreach (Match m in Regex.Matches(html ?? string.Empty, @"file:///[^\s'\""]*barcode_[A-Za-z0-9\-]+\.png", RegexOptions.IgnoreCase))
                {
                    tempImageUris.Add(m.Value);
                }

                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var document = new Document(); // tamaño por defecto
                    var writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    // Normalizar HTML a XHTML
                    string fixedHtml = NormalizeHtmlToXhtml(html ?? string.Empty);

                    // Parsear HTML con XMLWorker (permitir que cargue <img src="file:///...png">)
                    try
                    {
                        using (var sr = new StringReader(fixedHtml))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"XMLWorker error: {ex.Message}");
                        // Fallback simple
                        try
                        {
                            using (var sr = new StringReader(html ?? string.Empty))
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
                // Intentar eliminar archivos temporales generados para el barcode
                foreach (var uri in tempImageUris)
                {
                    try
                    {
                        var localPath = new Uri(uri).LocalPath;
                        if (File.Exists(localPath))
                        {
                            File.Delete(localPath);
                        }
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

                using (var document = PdfDocument.Load(pdfPath))
                using (var printDocument = document.CreatePrintDocument())
                {
                    var printerSettings = new PrinterSettings();

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
                    printDocument.PrintController = new StandardPrintController();
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
    }
}
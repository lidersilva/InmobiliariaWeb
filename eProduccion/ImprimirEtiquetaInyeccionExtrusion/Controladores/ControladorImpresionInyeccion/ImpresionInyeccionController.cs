using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using ZXing;
using ZXing.Common;
using eProduccion.Data;

namespace eProduccion.ImprimirEtiquetaInyeccionExtrusion.Controladores.ControladorImpresionInyeccion
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
    public class ImpresionInyeccionController
    {
        private float MmToPoints(float mm) => mm / 25.4f * 72f;

        public string EnsureEan13(string input)
        {
            var digits = new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length >= 12) digits = digits.Substring(0, 12);
            else digits = digits.PadLeft(12, '0');

            int checksum = CalculateEan13Checksum(digits);
            return digits + checksum.ToString();
        }

        private int CalculateEan13Checksum(string twelveDigits)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int d = twelveDigits[i] - '0';
                sum += (i % 2) == 1 ? d * 3 : d;
            }
            int mod = sum % 10;
            return (10 - mod) % 10;
        }

        public string GenerateEan13BarcodeBase64(string ean13)
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
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                                     ImageLockMode.WriteOnly, bitmap.PixelFormat);
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

        public string SaveBarcodePng(string ean13)
        {
            try
            {
                string base64 = GenerateEan13BarcodeBase64(ean13);
                if (string.IsNullOrWhiteSpace(base64)) return null;

                byte[] bytes = Convert.FromBase64String(base64);
                string outDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
                if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

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

        public string GenerateHtmlForTicket(TicketData t)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "TicketInyeccion.html");
            if (!File.Exists(templatePath)) throw new FileNotFoundException($"Template no encontrado: {templatePath}");

            string html = File.ReadAllText(templatePath);

            string ean = EnsureEan13(t.CodigoArticulo ?? string.Empty);
            string barcodeFile = SaveBarcodePng(ean);
            string filePathForHtml = barcodeFile?.Replace("\\", "/") ?? string.Empty;

            string imgTag = !string.IsNullOrEmpty(filePathForHtml)
                ? $"<img src=\"{filePathForHtml}\" class=\"barcode\" style=\"display:block;margin:2px auto;max-width:60mm;height:auto;max-height:12mm;\" />"
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

        public string NormalizeHtmlToXhtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            html = Regex.Replace(html, @"<meta\b([^>]*)>", m =>
            {
                var s = m.Value;
                if (s.EndsWith("/>")) return s;
                return s.TrimEnd('>') + " />";
            }, RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"<img\b([^>]*)>", m =>
            {
                var s = "<img" + m.Groups[1].Value;
                if (s.EndsWith("/")) return s + ">";
                if (s.EndsWith("/>")) return s + ">";
                return s + " />";
            }, RegexOptions.IgnoreCase);

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

            return html;
        }

        public string CrearPdfDesdeHtml_ConCss(string html, float widthMm = 75f, float heightMm = 25f)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");
            if (!Directory.Exists(pdfFolder)) Directory.CreateDirectory(pdfFolder);

            string outputPath = Path.Combine(pdfFolder, $"ticket_{Guid.NewGuid()}.pdf");

            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                float w = MmToPoints(widthMm);
                float h = MmToPoints(heightMm);

                var pageSize = new iTextSharp.text.Rectangle(w, h);
                var document = new Document(pageSize, 0, 0, 0, 0);
                var writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                var compactCss = $@"<style>
                        @page {{ size: {widthMm}mm {heightMm}mm; margin: 0; }}
                        html, body {{ margin:0; padding:0; font-family: Arial, Helvetica, sans-serif; font-size:6px; color:#000; }}
                        table {{ width:100%; border-collapse:collapse; table-layout:fixed; font-size:6px; }}
                        td {{ padding:1px 2px; vertical-align:middle; }}
                        img.barcode {{ display:block; margin:1px auto; max-width:60mm; max-height:{Math.Round(heightMm * 0.3, 2)}mm; height:auto; }}
                        </style>";

                string htmlSafe = Regex.Replace(html ?? string.Empty, @"<img[^>]*src=['""]data:[^'""]+['""][^>]*>", "", RegexOptions.IgnoreCase);
                string htmlWithCss = compactCss + htmlSafe;
                string xhtml = NormalizeHtmlToXhtml(htmlWithCss);

                using (var sr = new StringReader(xhtml))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                }

                document.Close();
            }

            return outputPath;
        }

        public void PrintPdfViaShell(string pdfPath, string printerName = null)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = pdfPath,
                    Verb = string.IsNullOrWhiteSpace(printerName) ? "print" : "printto",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                if (!string.IsNullOrWhiteSpace(printerName))
                    psi.Arguments = $"\"{printerName}\"";

                using (var p = Process.Start(psi))
                {
                    p?.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error imprimiendo por shell: {ex.Message}");
            }
        }

        public void OpenPdfWithDefaultApp(string pdfPath)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", pdfPath);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", pdfPath);
                else
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo abrir el visor de PDF: {ex.Message}");
            }
        }
    }
}
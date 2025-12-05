using DinkToPdf;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Path = System.IO.Path;
using PdfDocument = PdfiumViewer.PdfDocument;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Runtime.InteropServices;


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

            string pdfPath = CrearPdfDesdeHtml_iTextSharp(html);

            PrintPdf(pdfPath);
        }
        public string GenerarHtmlTicket(TicketData t)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "ticket.html");
            string html = File.ReadAllText(templatePath);

            html = html.Replace("{{NRO_OP}}", t.NumeroOT.ToString())
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
            // Carpeta de salida
            string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdf");

            // Crear carpeta si no existe
            if (!Directory.Exists(pdfFolder))
                Directory.CreateDirectory(pdfFolder);

            // Nombre completo del archivo
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

        public void PrintPdf(string pdfPath, string printerName = null, bool abrirEnVisor = true)
        {
            try
            {
                // Validar ruta
                if (string.IsNullOrWhiteSpace(pdfPath))
                    throw new ArgumentException("La ruta del PDF no puede estar vacía.");

                if (!File.Exists(pdfPath))
                    throw new FileNotFoundException($"No se encontró el archivo PDF en la ruta: {pdfPath}");

                // Si se pide abrir en el visor predeterminado
                if (abrirEnVisor)
                {
                    OpenPdfWithDefaultApp(pdfPath);
                    return;
                }

                // Cargar documento PDF y enviar a impresora
                using (var document = PdfDocument.Load(pdfPath))
                using (var printDocument = document.CreatePrintDocument())
                {
                    var printerSettings = new PrinterSettings();

                    if (!string.IsNullOrWhiteSpace(printerName))
                    {
                        try
                        {
                            printerSettings.PrinterName = printerName; // impresora específica
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Nombre de impresora inválido '{printerName}': {ex.Message}");
                        }
                    }

                    printDocument.PrinterSettings = printerSettings;

                    // Evita mostrar el cuadro de diálogo de impresión
                    printDocument.PrintController = new StandardPrintController();

                    // Ejecutar impresión
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                // Logging defensivo
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
    }
}
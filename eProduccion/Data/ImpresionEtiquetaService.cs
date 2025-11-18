using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfSharp.Pdf.IO;
using System.Drawing;
using System.Drawing.Printing;
using Windows.Storage;
using ZXing;
using ZXing.QrCode;

namespace eProduccion.Data
{
    public class ImpresionEtiquetaService
    {
        public async Task ImprimirEtiquetaInyeccionExtrusion(bool imprimirNoConformes, int nroOT, string nroCaja, string codArticulo, int cantAprobadas, int cantRetenidas, int cantRechReciclable,
            int cantRechNoReciclable, string maquina, string operador, DateTime? fecha, string turno)
        {
            int cantidad = imprimirNoConformes ? cantRetenidas + cantRechReciclable + cantRechNoReciclable : cantAprobadas;


        }
        private void creacionTicket(int cantidadImpre, string Cliente, string Cliente2, string PerfilAcabado,int Cantidad, int NPedido, int DocEntryTabla, int cantidadEnPaquete, bool esrestante)
        {
            //var perfil = new Perfil();
            var fechaHoy = DateTime.Now;
            var CodMadre = (string?)null;
            var Color = string.Empty;
            double Longitud = 0.0;
            var Tipo = string.Empty;




            // Obtiene codigo de la primera etapa segun la hoja de ruta del perfil acabado que figura en su lista de materiales
            //var primeraEtapaAcabado = perfil.ObtenerPrimerEstacionAcabado(PerfilAcabado);
            int docEntry = 0;
            //var embalado = new ProduccionPerfilesWeb.App_Data.Embalado();
            string NOt = null;
            string Lote = null;
            string cantidadEnPaqueteStr = cantidadEnPaquete.ToString();

            //if (primeraEtapaAcabado == "EANODIZACION" || primeraEtapaAcabado == "ELACADO")
            //{
            //    //docEntry = embalado.ObtenerDocEntryOT_CARDES_CAB(NPedido, PerfilAcabado);
            //}
            //else if (primeraEtapaAcabado == "EEMBALADO")
            //{
            //    docEntry = DocEntryTabla;
            //}
            if (docEntry > 0)
            {
                NOt = docEntry.ToString();
                Lote = docEntry.ToString() + "-" + DateTime.Now.ToString("ddMMyyyy");
            }

            GenerateBarCode(PerfilAcabado);
            #region TICKET
            // Valor que deseas cargar
            double longitud = Longitud;
            string valorlogitud = longitud.ToString();
            int pedido = NPedido;
            string valorpedido = pedido.ToString();
            var fecha = fechaHoy.ToString("dd/MM/yyyy");
            string valorfecha = fecha.ToString();

            int espaciofaltantecliente2;
            int espaciocodmadre;
            int espaciocolor;
            int espaciocantidad;
            int espaciologitud;


            #region Validaciones
            if (Cliente is null)
            {
                Cliente = "N/A";
            }

            if (Cliente2 is null)
            {
                Cliente2 = "N/A";
            }

            //if (CodMadre is null)
            //{
            //    CodMadre = "N/A";
            //}

            //if (Color is null)
            //{
            //    Color = "N/A";
            //}

            if (cantidadEnPaqueteStr is null)
            {
                cantidadEnPaqueteStr = "N/A";
            }

            if (valorlogitud is null)
            {
                valorlogitud = "N/A";
            }

            if (valorpedido is null)
            {
                valorpedido = "N/A";
            }

            if (NOt is null)
            {
                NOt = "N/A";
            }


            if (fecha is null)
            {
                fecha = "N/A";
            }

            if (Lote is null)
            {
                Lote = "N/A";
            }

            //if (Tipo is null)
            //{
            //    Tipo = "N/A";
            //}
            #endregion
            espaciofaltantecliente2 = Math.Max(0, 10 - Cliente2.Length);
            espaciocodmadre = Math.Max(0, 15 - CodMadre.Length);
            espaciocolor = Math.Max(0, 10 - Color.Length);
            espaciocantidad = Math.Max(0, 10 - cantidadEnPaqueteStr.Length);
            espaciologitud = Math.Max(0, 10 - valorlogitud.Length);



            // Formato del ticket
            string formatoTicket = @"


                                                    ANTES DE LA 
                                                    INSTALACION:
                                                    Proteger de la humedad y
                                                    contaminación ambiental
                                                    Evitar par galvánico
            CLIENTE:   {cliente}
            CLIENTE 2: {cliente2}       N° PEDIDO: {nPedido}
            COD MADRE: {codMadre}N° OT: {nOt}
            TIPO: {tipo}               FECHA: {fechaHoy}
            CANTIDAD : {cantidad}       LOTE: {lote}
            LONGITUD : {longitud}       COLOR:{color}


                                               
                           
                    Producido en Villeta - Paraguay";
            Cliente2 = Cliente2.Length > 10 ? Cliente2.Substring(0, 10) : Cliente2;
            CodMadre = CodMadre.Length > 10 ? CodMadre.Substring(0, 10) : CodMadre;
            // Color = Color.Length > 9 ? Color.Substring(0, 9) : Color;
            cantidadEnPaqueteStr = cantidadEnPaqueteStr.Length > 13 ? cantidadEnPaqueteStr.Substring(0, 10) : cantidadEnPaqueteStr;
            valorlogitud = valorlogitud.Length > 10 ? valorlogitud.Substring(0, 10) : valorlogitud;
            // Dividir el formato en líneas
            string[] lineas = formatoTicket.Split('\n');

            // Reemplazar etiquetas en cada línea
            for (int i = 0; i < lineas.Length; i++)
            {
                lineas[i] = lineas[i]
                    .Replace("{cliente}", Cliente)
                    .Replace("{cliente2}", Cliente2.PadRight(Cliente2.Length + espaciofaltantecliente2))
                    .Replace("{codMadre}", CodMadre.PadRight(CodMadre.Length + espaciocodmadre))
                    .Replace("{color}", Color/*.PadRight(Color.Length + espaciocolor)*/)
                    .Replace("{cantidad}", cantidadEnPaqueteStr.PadRight(cantidadEnPaqueteStr.Length + espaciocantidad))
                    .Replace("{longitud}", valorlogitud.PadRight(valorlogitud.Length + espaciologitud))
                    .Replace("{nPedido}", valorpedido.PadRight(30))
                    .Replace("{nOt}", NOt.PadRight(30))
                    .Replace("{fechaHoy}", valorfecha.PadRight(30))
                    .Replace("{lote}", Lote.PadRight(30))
                    .Replace("{tipo}", Tipo.PadRight(30));
            }

            // Unir las líneas de nuevo
            string resultado = string.Join('\n', lineas);


            #endregion

            Imprimir(resultado, cantidadImpre, DocEntryTabla, esrestante, PerfilAcabado);

        }
        public static async Task GenerateBarCode(string data)
        {
            var barCodeData = new BarcodeWriterPixelData
            {
                //Format = BarcodeFormat.CODE_128,
                Format = BarcodeFormat.CODE_39,
                Options = new QrCodeEncodingOptions
                {
                    Height = 40,
                    Width = 240,
                    Margin = 6,
                    CharacterSet = data
                }
            };

            var pixelData = barCodeData.Write(data);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);

                    bitmap.UnlockBits(bitmapData);
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                    string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcode.png");

                    await System.IO.File.WriteAllBytesAsync(path, memoryStream.ToArray()); // Usa System.IO.File aquí
                }
            }
        }
        private void Imprimir(string contenido, int cantidadImpre, int DocEntryTabla, bool esrestante, string perfilacabado)
        {
            // Obtén la ruta completa de la imagen dentro de la carpeta del proyecto
            string nombreImagen = "ALUKLER.jpeg";
            string rutaImagen = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", nombreImagen);
            string pathtodirectory = KnownFolders.LocalAppData.Path; // verificar
            string pathtopdf = $"{Directory.GetCurrentDirectory()}\\wwwroot\\pdf\\";
            Int16 shortcantImpre = (Int16)(cantidadImpre);
            PrinterSettings ps = new PrinterSettings();
            PrintDocument printDocument = new PrintDocument();
            //PdfDocument printDocument = new PdfDocument();
            PageSettings pageSettings = new PageSettings();
            PaperSize paperSize = new PaperSize("EtiquetaEmbalado", 354, 276);
            paperSize.RawKind = 0;
            paperSize.Width = 354;
            paperSize.Height = 276;
            pageSettings.PaperSize = paperSize;
            ps.DefaultPageSettings.PaperSize = paperSize;
            printDocument.PrinterSettings = ps;
            //printDocument.PrinterSettings.Copies = shortcantImpre;
            //printDocument.PrinterSettings.IsDefaultPrinter = true;
            printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            printDocument.PrinterSettings.PrintToFile = true;
            if (!esrestante)
            {
                printDocument.PrinterSettings.PrintFileName = pathtopdf + "/ticket.pdf";
            }
            else
            {
                printDocument.PrinterSettings.PrintFileName = pathtopdf + "/ticket2.pdf";
            }

            printDocument.DefaultPageSettings = pageSettings;
            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            printDocument.PrintPage += (sender, args) =>
            {
                args.Graphics.DrawString(contenido, new System.Drawing.Font("Arial", 10), Brushes.Black, 10, 10);
                // Agregar la imagen del código de barras después de imprimir el contenido del ticket
                string pathToBarcodeImage = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "barcode.png");
                if (System.IO.File.Exists(pathToBarcodeImage))
                {
                    System.Drawing.Image barcodeImage = System.Drawing.Image.FromFile(pathToBarcodeImage);
                    args.Graphics.DrawImage(barcodeImage, new PointF(50, 240)); // Ajusta la posición según tus necesidades
                }
                // Agregar la imagen desde la carpeta del proyecto
                if (System.IO.File.Exists(rutaImagen))
                {
                    System.Drawing.Image additionalImage = System.Drawing.Image.FromFile(rutaImagen);
                    args.Graphics.DrawImage(additionalImage, new PointF(30, 60)); // Ajusta la posición según tus necesidades
                }
                else
                {
                    Console.WriteLine("La imagen no se encuentra en la ruta especificada: " + rutaImagen);
                }
                args.Graphics.DrawString(perfilacabado, new System.Drawing.Font("Arial", 10), Brushes.Black, 150, 283);

                //no se pueden cambiar las dimensiones, el documento se guarda con los valores predeterminados por el driver de Microsoft Print to PDF
                //por eso usamos librerias para manipular el documento generado
                //args.PageSettings.PaperSize.RawKind = 0;
                //args.PageSettings.PaperSize.Height = 276;
                //args.PageSettings.PaperSize.Width = 354;
            };


            // Imprimir el contenido del ticket
            try
            {
                printDocument.Print();

                //manipulamos el pdf generado para poder imprimir en el formato deseado por el cliente
                string sourceFile = pathtopdf + "/ticket.pdf";
                string sourceFileTrimmed = pathtopdf + "/ticketTrimmed.pdf";
                string sourceFileScaled = pathtopdf + "/ticketScaled.pdf";
                string sourceFile2 = pathtopdf + "/ticket2.pdf";
                string sourceFile2Trimmed = pathtopdf + "/ticket2Trimmed.pdf";
                string sourceFile2Scaled = pathtopdf + "/ticket2Scaled.pdf";
                string sourceFileMerged = pathtopdf + "/mergedTicket.pdf";
                if (esrestante)
                {
                    trimPdfPages(sourceFile2, sourceFile2Trimmed);
                    //ScaleToCustomSize(sourceFile2Trimmed, sourceFile2Scaled);

                    PdfSharp.Pdf.PdfDocument outputDocument = new PdfSharp.Pdf.PdfDocument();
                    PdfSharp.Pdf.PdfDocument pdf2 = PdfSharp.Pdf.IO.PdfReader.Open(sourceFile2Trimmed, PdfDocumentOpenMode.Import);

                    if (cantidadImpre > 1)
                    {
                        PdfSharp.Pdf.PdfDocument pdf3 = PdfSharp.Pdf.IO.PdfReader.Open(sourceFileMerged, PdfDocumentOpenMode.Import);
                        int count = pdf3.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfSharp.Pdf.PdfPage page3 = pdf3.Pages[idx];
                            outputDocument.AddPage(page3);
                        }
                    }

                    PdfSharp.Pdf.PdfPage page2 = pdf2.Pages[0];
                    outputDocument.AddPage(page2);

                    outputDocument.Save(sourceFileMerged);
                    try
                    {
                        System.IO.File.Delete(sourceFile2Trimmed);
                    }
                    catch (IOException ioExp)
                    {
                        Console.WriteLine(ioExp.Message);
                    }
                }
                else
                {
                    trimPdfPages(sourceFile, sourceFileTrimmed);
                    //ScaleToCustomSize(sourceFileTrimmed, sourceFileScaled);

                    PdfSharp.Pdf.PdfDocument pdf = PdfSharp.Pdf.IO.PdfReader.Open(sourceFileTrimmed);
                    for (int i = 0; i < cantidadImpre - 2; i++)
                    {
                        pdf.AddPage((PdfSharp.Pdf.PdfPage)pdf.Pages[0].Clone());
                    }
                    pdf.Save(sourceFileMerged);

                    try
                    {
                        System.IO.File.Delete(sourceFileTrimmed);
                    }
                    catch (IOException ioExp)
                    {
                        Console.WriteLine(ioExp.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error de impresión
                Console.WriteLine("Error de impresión: " + ex.Message);
            }
            finally
            {
                printDocument.Dispose();
            }
        }

        //creamos un nuevo pdf solamente con el contenido del pdf previamente generado
        public void trimPdfPages(string resourceStream, string outputFilePath)
        {
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(resourceStream);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.Write));

            int n = reader.NumberOfPages;
            for (int i = 1; i <= n; i++)
            {
                iTextSharp.text.Rectangle pageSize = reader.GetPageSize(i);
                iTextSharp.text.Rectangle rect = getOutputPageSize(pageSize, reader, i);

                PdfDictionary page = reader.GetPageN(i);
                page.Put(PdfName.CROPBOX, new PdfArray(new float[] { rect.Left, rect.Bottom, rect.Right, rect.Top }));
                stamper.MarkUsed(page);
            }
            stamper.Close();
        }

        //obtenemos las dimensiones del pdf generado dejando de lado los espacios blancos fuera del contenido
        private iTextSharp.text.Rectangle getOutputPageSize(iTextSharp.text.Rectangle pageSize, iTextSharp.text.pdf.PdfReader reader, int page)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            TextMarginFinder finder = parser.ProcessContent(page, new TextMarginFinder());
            iTextSharp.text.Rectangle result = new iTextSharp.text.Rectangle(finder.GetLlx(), finder.GetLly(), finder.GetUrx(), finder.GetUry());
            return result;
        }
    }
}

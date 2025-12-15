using eProduccion.Controladores;

namespace eProduccion.Data
{
    public class ImpresionEtiquetaService
    {
        private readonly ImpresionInyeccionController _impresionController;

        public ImpresionEtiquetaService()
        {
            _impresionController = new ImpresionInyeccionController();
        }
        public async Task ImprimirEtiquetaInyeccionExtrusion(
            bool imprimirNoConformes,
            int nroOT,
            string nroCaja,
            string codArticulo,
            int cantAprobadas,
            int cantRetenidas,
            int cantRechReciclable,
            int cantRechNoReciclable,
            string maquina,
            string operador,
            DateTime? fecha,
            string turno)
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

            string html = _impresionController.GenerarHtmlTicket(ticket);
            string pdfPath = _impresionController.CrearPdfDesdeHtml_ConCss(html);

            _impresionController.PrintPdf(pdfPath);
        }
    }
}

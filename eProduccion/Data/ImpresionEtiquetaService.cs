using eProduccion.Controladores;
using static eProduccion.Controladores.ImpresionEmsambleController;

namespace eProduccion.Data
{
    public class ImpresionEtiquetaService
    {
        private readonly ImpresionInyeccionController _impresioninyeController;
        private readonly ImpresionEmsambleController _impresionemsamController;
        public ImpresionEtiquetaService()
        {
            _impresioninyeController = new ImpresionInyeccionController();
            _impresionemsamController = new ImpresionEmsambleController();
        }
        public async Task ImprimirEtiquetaInyeccionExtrusion(
            string tipoEtiqueta,
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
            int cantidadImprimir = tipoEtiqueta == "Aprobados" ? cantAprobadas : cantRechReciclable + cantRechNoReciclable;

            var ticket = new TicketDataInyeccion
            {
                NumeroOT = nroOT,
                NumeroCaja = nroCaja,
                CodigoArticulo = codArticulo,
                CantidadAprobadas = cantidadImprimir,
                Maquina = maquina,
                Operador = operador,
                Fecha = fecha ?? DateTime.Now,
                Turno = turno
            };

            string html = _impresioninyeController.GenerarHtmlTicket(ticket);
            string pdfPath = _impresioninyeController.CrearPdfDesdeHtml_ConCss(html);

            _impresioninyeController.PrintPdf(pdfPath);
        }

        public Task ImprimirEtiquetaEmsamble(
           string operador,
           DateTime? fechaRecepcionSE,
           DateTime? horaRecepcionSE,
           string producto,
           DateTime? fechaFinArmado,
           DateTime? horaFinArmado,
           double cantidadEntregada)
        {
            var ticketEmbalado = new TicketDataEmsamble
            {
                Operador = operador,
                FechaRecepcion = fechaRecepcionSE ?? DateTime.Now,
                HoraRecepcion = horaRecepcionSE ?? DateTime.Now,
                Producto = producto,
                FechaFin = fechaFinArmado ?? DateTime.Now,
                HoraFin = horaFinArmado ?? DateTime.Now,
                CantidadEntregado = cantidadEntregada
            };

            string html = _impresionemsamController.GenerarHtmlTicket(ticketEmbalado);
            string pdfPath = _impresionemsamController.CrearPdfDesdeHtml_ConCss(html);

            _impresionemsamController.PrintPdf(pdfPath);

            return Task.CompletedTask;
        }
    }
}

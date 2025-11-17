namespace eProduccion.Data
{
    public class ImpresionEtiquetaService
    {
        public async Task ImprimirEtiquetaInyeccionExtrusion(bool imprimirNoConformes, int nroOT, string nroCaja, string codArticulo, int cantAprobadas, int cantRetenidas, int cantRechReciclable,
            int cantRechNoReciclable, string maquina, string operador, DateTime? fecha, string turno)
        {
            int cantidad = imprimirNoConformes ? cantRetenidas + cantRechReciclable + cantRechNoReciclable : cantAprobadas;


        }
    }
}

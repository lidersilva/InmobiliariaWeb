namespace eProduccion.Models
{
    public class OTInyeccion
    {
        public int DocEntry {  get; set; }
        public DateTime FechaOT { get; set; } 
        public string CodArticuloOV { get; set; }
        public string ArticuloOV { get; set; }
        public string CodArticuloI {  get; set; }
        public string ArticuloI {  get; set; }
        public double CantidadOT { get; set; }
        public int DocNumOV { get; set; }
        public string SerieOV { get; set; }
        public string EstadoOT {  get; set; }
        public int CavidadesReales {  get; set; }
        public List<OTInyeccionDet> OTInyeccionDetalle {  get; set; }
    }
}

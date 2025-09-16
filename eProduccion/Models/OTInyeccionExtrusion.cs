namespace eProduccion.Models
{
    public class OTInyeccionExtrusion
    {
        public int DocEntry {  get; set; }
        public DateTime FechaOT { get; set; } 
        public string CodArticuloOV { get; set; }
        public string ArticuloOV { get; set; }
        public string CodArticuloIE {  get; set; }
        public string ArticuloIE {  get; set; }
        public double CantidadOT { get; set; }
        public int DocNumOV { get; set; }
        public string SerieOV { get; set; }
        public string EstadoOT {  get; set; }
        public int CavidadesReales {  get; set; }
        public int CodePlanificacionOT { get; set; }
        public List<OTInyeccionExtrusionDet> OTInyeccionExtrusionDetalle {  get; set; }
    }
}

namespace eProduccion.Models
{
    public class OTEnsamble
    {
        public int DocEntry { get; set; }
        public DateTime FechaOT { get; set; }
        public string CodArticuloOV { get; set; }
        public string ArticuloOV { get; set; }
        public string CodArticuloEnsamble { get; set; }
        public string ArticuloEnsamble { get; set; }
        public double CantidadOT { get; set; }
        public int DocNumOV { get; set; }
        public string SerieOV { get; set; }
        public string EstadoOT { get; set; }
        public int CodePlanificacionOT { get; set; }
    }
}

namespace eProduccion.Models
{
    public class PlanificacionEnsamble
    {
        public int DocEntry { get; set; }
        public int CodePlaniOT { get; set; }
        public int DocNumOV { get; set; }
        public DateTime FechaOV { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public double CantidadOV { get; set; }
        public string Serie { get; set; }
        public List<PlanificacionEnsambleDet> PlanificacionEnsambleDetalle { get; set; }
    }
}

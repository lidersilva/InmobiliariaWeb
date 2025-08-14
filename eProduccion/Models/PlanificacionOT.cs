namespace eProduccion.Models
{
    public class PlanificacionOT
    {
        public int Code { get; set; }
        public int DocNumOV { get; set; }
        public string EstadoOT { get; set; }
        public DateTime FechaOV { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public double CantidadOV { get; set; }
        public DateTime FechaOT { get; set; }
        public DateTime HoraOT { get; set; }
        public string UsuarioOT { get; set; }
        public double CantProducida { get; set; }
        public string Serie { get; set; }
    }
}

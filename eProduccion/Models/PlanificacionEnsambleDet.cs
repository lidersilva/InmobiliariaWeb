namespace eProduccion.Models
{
    public class PlanificacionEnsambleDet
    {
        public string SerieOV { get; set; }
        public int DocNumOV { get; set; }
        public DateTime FechaOV { get; set; }
        public int CantProducida { get; set; }
        public double CantProducidaKG { get; set; }
        public int CantSolicitar { get; set; }
        public double CantSolicitada { get; set; }
        public double CantDisponible { get; set; }
        public string Lote { get; set; }
        public int OT { get; set; }
        public string CodArticuloI { get; set; }
        public string ArticuloI { get; set; }
        public double Stock {  get; set; }
        public string Estado { get; set; }
    }
}

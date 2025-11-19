namespace eProduccion.Models
{
    public class PendienteMolinar
    {
        public int DocEntry { get; set; }
        public string EstacionOrigen { get; set; }
        public DateTime Fecha { get; set; }
        public string Turno { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string Operario { get; set; }
        public string Operario2 { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public string Lote { get; set; }
        public int CantProducida { get; set; }
        public int CantSolicitar { get; set; }
        public int CantSolicitada { get; set; }
        public int CantDisponible { get; set; }
        public string UMArticulo { get; set; }
    }
}

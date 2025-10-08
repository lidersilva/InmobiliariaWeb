namespace eProduccion.Models
{
    public class OTEnsambleDet
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string NroContenedor { get; set; }
        public string NroMaquina { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string Turno { get; set; }
        public string Operario { get; set; }
        public Usuario? UsuarioOperario { set; get; }
        public string Operario2 { get; set; }
        public Usuario? UsuarioOperario2 { set; get; }
        public int CantAprobadas { get; set; }
        public double CantAprobadasKG { get; set; }
        public int CantAprobadasDesvio { get; set; }
        public double PesoPiezaG { get; set; }
        public bool Liberado { get; set; }
        public int DocEntryEntrada { get; set; }
        public int DocEntrySalida { get; set; }
        public int DocNumEntrada { get; set; }
        public int DocNumSalida { get; set; }
        public int Asiento { get; set; }
        public string EstadoLinea { get; set; }
        public string Observaciones { get; set; }
    }
}

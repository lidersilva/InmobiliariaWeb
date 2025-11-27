namespace eProduccion.Models
{
    public class OTMolino
    {
        public int DocEntry { get; set; }
        public int DocEntryPendienteMoli { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string Turno { get; set; }
        public string Operario { get; set; }
        public Usuario? UsuarioOperario { set; get; }
        public string Operario2 { get; set; }
        public Usuario? UsuarioOperario2 { set; get; }
        public int CantProcesar { get; set; }
        public double CantReciclableKG { get; set; }
        public double CantNoConformeKG { get; set; }
        public string MotiRechazo { get; set; }
        public MotivoParadaDefecto? MotRechazo { get; set; }
    }
}

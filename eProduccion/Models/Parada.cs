namespace eProduccion.Models
{
    public class Parada
    {
        public int DocEntry { get; set; }
        public string Estacion { get; set; }
        public int DocEntryOT { get; set; }
        public int LineIdOT { get; set; }
        public DateTime? Fecha { get; set; }
        public string NroMaquina { get; set; }
        public string TipoParada { get; set; }
        public string Turno { get; set; }
        public string Operador1 { get; set; }
        public string Operador2 { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
    }
}

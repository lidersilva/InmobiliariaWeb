namespace eProduccion.Models
{
    public class OTInyeccionExtrusionDet
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
        public int CantAprobadas { get; set; }
        public int CantRetenidas { get; set; }
        public int CantRechReciclable { get; set; }
        public double PesoRechReciclable { get; set; }
        public string MotiMPesoRechReciclable { get; set; }
        public MotivoParadaDefecto? MotPesoRechReciclable { get; set; }
        public int CantRechNoReciclable { get; set; }
        public double PesoRechNoReciclable { get; set; }
        public string MotiMPesoRechNoReciclable { get; set; }
        public MotivoParadaDefecto? MotPesoRechNoReciclable { get; set; }
        public double PesoColadaKG { get; set; }
        public double PesoMasacoteKG { get; set; }
        public double PesoAjusMaquinaKG { get; set; }
        public double PesoPiezaG { get; set; }
        public int CavidadReal { get; set; }
        public int CavidadOperativa { get; set; }
        public double TiempoCicloReal { get; set; }
        public double TiempoCiclo { get; set; }
        public bool Liberado { get; set; }
        public int DocEntryEntrada { get; set; }
        public int DocEntrySalida { get; set; }
        public int DocNumEntrada { get; set; }
        public int DocNumSalida { get; set; }
        public int Asiento { get; set; }
        public string EstadoLinea { get; set; }
    }
}

namespace eProduccion.Models
{
    public class Parametrizacion
    {
        public int? DocEntry { get; set; }
        public string CtaProduccionCurso { get; set; }
        public string AlmacenSalidaIny { get; set; }
        public string AlmacenAprobadosIny { get; set; }
        public string AlmacenRechReciIny { get; set; }
        public string AlmacenRechNoReciIny { get; set; }
        public string AlmacenRetenidosIny { get; set; }
        public string AlmacenSalidaExt { get; set; }
        public string AlmacenAprobadosExt { get; set; }
        public string AlmacenRechReciExt { get; set; }
        public string AlmacenRechNoReciExt { get; set; }
        public string AlmacenRetenidosExt { get; set; }
        public List<SerieDetalle> SerieDetalle { get; set; }
    }
}

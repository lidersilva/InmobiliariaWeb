namespace eProduccion.Models
{
    public class RegistroEnsambleDet
    {
        public int LineId { get; set; }
        public string CodArticulo { get; set; }
        public string Articulo { get; set; }
        public string Lote { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public int CantidadReproceso { get; set; }
        public bool LineaNueva { get; set; } = false;
    }
}

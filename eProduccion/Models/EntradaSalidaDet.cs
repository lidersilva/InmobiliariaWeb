namespace eProduccion.Models
{
    public class EntradaSalidaDet
    {
        public string Articulo {  get; set; }
        public double Cantidad { get; set; }
        public List<Lote> LoteDetalle {  get; set; }
    }
}

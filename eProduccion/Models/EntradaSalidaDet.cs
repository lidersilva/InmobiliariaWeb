namespace eProduccion.Models
{
    public class EntradaSalidaDet
    {
        public string Articulo {  get; set; }
        public double Cantidad { get; set; }
        public string Almacen {  get; set; }
        public double PrecioUnitario { get; set; }
        public List<Lote> LoteDetalle {  get; set; }
    }
}

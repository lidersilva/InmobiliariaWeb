namespace eProduccion.Models
{
    public class Rol
    {
        public string Codigo { get; set; }
        public string Descripcion {  get; set; }
        public string Activo { get; set; }
        public List<RolDetalle> RolDetalle { get; set; }
    }
}

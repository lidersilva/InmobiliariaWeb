namespace eProduccion.Models
{
    public class Usuario
    {
        public int Code { get; set; }
        public string? CodigoUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Email { get; set; }
        public string? Estado { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? TipoUsuario { get; set; }
    }
}
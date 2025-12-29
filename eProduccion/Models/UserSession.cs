namespace eProduccion.Models
{
    public class UserSession
    {
        public string UserName { get; set; }
        public string PassSecure { get; set; }
        public string DataBase { get; set; }
        public string CompanyName { get; set; }
        public HashSet<string> Permisos { get; set; } = [];
        public Session SapSession { get; set; }

        public bool TienePermiso(string permiso)
        {
            return Permisos?.Contains(permiso) == true;
        }

        public void Clear()
        {
            UserName = null;
            PassSecure = null;
            DataBase = null;
            CompanyName = null;
            Permisos.Clear();
            SapSession = null;
        }
    }
}
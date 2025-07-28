namespace eProduccion.Models
{
    public class UserObjectsMD
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ObjectType { get; set; }
        public static class TipoObjeto
        {
            public static string DatoMaestro = "boud_MasterData";
            public static string Documento = "boud_Document";
        }
        public string CanFind { get; set; }
        public string CanCancel { get; set; }
        public string CanDelete { get; set; }
        public string CanLog { get; set; }
        public string CanCreateDefaultForm { get; set; }
        public string EnableEnhancedForm { get; set; }
        public string RebuildEnhancedForm { get; set; }
        public List<UserChildTablesMD> ChildTables { get; set; }
        public List<UserColumnsMD_FindColumns> FindColumns { get; set; }
        public List<UserColumnsMD_FormColumns> FormColumns { get; set; }
    }
}

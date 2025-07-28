namespace eProduccion.Models
{
    public class UserFieldsMD
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public static class TipoCampo
        {
            public static string Alpha = "db_Alpha";
            public static string Memo = "db_Memo";
            public static string Numeric = "db_Numeric";
            public static string Date = "db_Date";
            public static string Float = "db_Float";
        }
        public int Size { get; set; }
        public string Description { get; set; }
        public string SubType { get; set; }
        public static class SubTipoCampo
        {
            public static string None = "st_None";
            public static string Address = "st_Address";
            public static string Phone = "st_Phone";
            public static string Time = "st_Time";
            public static string Rate = "st_Rate";
            public static string Sum = "st_Sum";
            public static string Price = "st_Price";
            public static string Quantity = "st_Quantity";
            public static string Percentage = "st_Percentage";
            public static string Measurement = "st_Measurement";
            public static string Link = "st_Link";
            public static string Image = "st_Image";
        }
        public string TableName { get; set; }
        public int FieldID { get; set; }
        public string LinkedTable { get; set; }
        public string DefaultValue { get; set; }
        public List<ValidValuesMD> ValidValuesMD { get; set; }
        public int? LinkedSystemObject { get; set; }
    }
}

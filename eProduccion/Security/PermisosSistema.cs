namespace eProduccion.Security
{
    public static class PermisosSistema
    {
        // Vistas Gestión accesos y Configuración
        public const string GestionAccesos = "01";
        public const string GenerarEstructura = "02";
        public const string Parametrizacion = "04";

        // Vistas Producción
        public const string PlanificacionOT = "10";
        public const string Lotes = "100";
        public const string Inyeccion = "11";
        public const string Extrusion = "12";
        public const string PlanificacionEnsamble = "13";
        public const string Armado = "14";
        public const string Flowpack = "15";
        public const string Sachetera = "16";
        public const string Sellado = "17";
        public const string Horneado = "18";
        public const string Empaquetado = "19";
        public const string Prensa = "20";
        public const string GrabadoLaser = "21";
        public const string PendienteMolinar = "22";
        public const string Molino = "23";
    }
}

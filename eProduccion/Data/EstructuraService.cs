using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using static eProduccion.Models.Consts;
using static eProduccion.Models.UserFieldsMD;
using static eProduccion.Models.UserObjectsMD;

namespace eProduccion.Data
{
    public class EstructuraService
    {
        private readonly ConnectionService _connectionService;

        public EstructuraService(ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task GenerarEstructura()
        {
            #region Crear tablas
            new List<UserTablesMD>
            {
                new () { TableName = "EEP_PERM", Descr = "EEP Maestro de permisos", ObjectType = 1 },
                new () { TableName = "EEP_ROLU", Descr = "EEP Roles_Usuarios", ObjectType = 1 },
                new () { TableName = "EEP_ROLC", Descr = "EEP Maestro de roles", ObjectType = 1 },
                new () { TableName = "EEP_ROLD", Descr = "EEP Roles_Permisos", ObjectType = 2 },
                new () { TableName = "EEP_PARAM", Descr = "EEP Parametrización", ObjectType = 3 },
                new () {TableName = "EEP_PARSERIE_DET", Descr = "EEP Param. series det.", ObjectType = 4},
                new () { TableName = "EEP_PLANI_OT", Descr = "EEP Planificación OT", ObjectType = 5 },
                new () { TableName = "EEP_OT_INYEX_CAB", Descr = "EEP OT Inyección/Extrusión cab", ObjectType = 3 },
                new () {TableName = "EEP_OT_INYEX_DET", Descr = "EEP OT Inyección/Extrusión det", ObjectType = 4},
                new () { TableName = "EEP_PARADAS", Descr = "EEP Registro paradas", ObjectType = 3 },
                new () { TableName = "EEP_PARADA_DEFECTO", Descr = "Maestro mot. paradas/defectos", ObjectType = 3 },
                new () { TableName = "EEP_ENSAM_CAB", Descr = "EEP Planif. ensamblado cab.", ObjectType = 3 },
                new () {TableName = "EEP_ENSAM_DET", Descr = "EEP Planif. ensamblado det.", ObjectType = 4},
                new () { TableName = "EEP_OT_ENSAM_CAB", Descr = "EEP OT Ensamblado cab.", ObjectType = 3 },
                new () {TableName = "EEP_OT_ENSAM_DET", Descr = "EEP OT Ensamblado det.", ObjectType = 4},
                new () {TableName = "EEP_REG_ENSAM_DET", Descr = "EEP Registro Ensamblado det.", ObjectType = 4},
                new () { TableName = "EEP_PEND_MOLI_OT", Descr = "EEP OT Pendiente a molinar", ObjectType = 3 },
                new () { TableName = "EEP_OT_MOLINO", Descr = "EEP OT Molino", ObjectType = 3 },
            }.ForEach(AgregarTablas);
            #endregion

            #region Crear campos
            var valoresValidosVacios = new List<ValidValuesMD>();// Lista vacía
            new List<UserFieldsMD>
            {
                // Gestión de Roles y Permisos
                new () { Name = "DESC", Type = TipoCampo.Alpha, Size = 100, Description = "Descripción", SubType = SubTipoCampo.None, TableName = "@EEP_PERM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "USER", Type = TipoCampo.Alpha, Size = 25, Description = "Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_ROLU", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ROLID", Type = TipoCampo.Alpha, Size = 10, Description = "Rol", SubType = SubTipoCampo.None, TableName = "@EEP_ROLU", LinkedTable = "EEP_ROLC", DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ACTI", Type = TipoCampo.Alpha, Size = 1, Description = "Activo", SubType = SubTipoCampo.None, TableName = "@EEP_ROLC", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "PERM", Type = TipoCampo.Alpha, Size = 1, Description = "Código de Permiso", SubType = SubTipoCampo.None, TableName = "@EEP_ROLD", LinkedTable = "EEP_PERM", DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TIPO", Type = TipoCampo.Alpha, Size = 2, Description = "Tipo usuario", SubType = SubTipoCampo.None, TableName = "@EEP_USUA", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // Parametrización
                new () { Name = "CTAPRODC", Type = TipoCampo.Alpha, Size = 15, Description = "Cuenta de producción en curso", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ASALIDAI", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. salida inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBI", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHRECII", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados reciclables inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHNORECII", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados no reciclables inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETENIDOSI", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ASALIDAE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. salida extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHRECIE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados reciclables extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHNORECIE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados no reciclables extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETENIDOSE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBAR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBFL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBSA", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBSE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBHO", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBEM", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBPR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AAPROBGL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. aprobados grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHAR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHFL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHSA", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHSE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHHO", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHEM", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHPR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECHGL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. rechazados grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUAR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUFL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUSA", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUSE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUHO", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUEM", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUPR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECUGL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. recuperados grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEAR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEFL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETESA", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETESE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEHO", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEEM", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEPR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARETEGL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. retenidos grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROAR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROFL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROSA", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROSE", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROHO", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROEM", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROPR", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "AREPROGL", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reprocesados grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CINYECCION", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación inyección", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CEXTRUSION", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación extrusión", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CARMADO", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación armado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CFLOWPACK", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación flowpack", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CSACHETERA", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación sachetera", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CSELLADO", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación sellado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CHORNEADO", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación horneado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CEMPAQUE", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación empaquetado", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CPRENSA", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación prensa", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CGRABADOL", Type = TipoCampo.Alpha, Size = 50, Description = "Código estación grabado láser", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ARECIMOLI", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. reciclables molino", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ANORECIMOLI", Type = TipoCampo.Alpha, Size = 8, Description = "Alm. no reciclables molino", SubType = SubTipoCampo.None, TableName = "@EEP_PARAM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                // Parametrización - Series Det.
                new () { Name = "CODSERIE", Type = TipoCampo.Numeric, Size = 11, Description = "Código serie", SubType = SubTipoCampo.None, TableName = "@EEP_PARSERIE_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "SERIE", Type = TipoCampo.Alpha, Size = 8, Description = "Serie", SubType = SubTipoCampo.None, TableName = "@EEP_PARSERIE_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // Planificación OT
                new () { Name = "DOCENTRYOV", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DOCNUMOV", Type = TipoCampo.Numeric, Size = 11, Description = "DocNum OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODSERIE", Type = TipoCampo.Numeric, Size = 11, Description = "Código serie OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAOV", Type = TipoCampo.Date, Size = 10, Description = "Fecha OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODARTICULO", Type = TipoCampo.Alpha, Size = 50, Description = "Código artículo", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTIDADOV", Type = TipoCampo.Float, Size = 10, Description = "Cantidad OV", SubType = SubTipoCampo.Quantity, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODCLIENTE", Type = TipoCampo.Alpha, Size = 50, Description = "Cliente OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado OT", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAOT", Type = TipoCampo.Date, Size = 10, Description = "Fecha inicio OT", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAOT", Type = TipoCampo.Date, Size = 8, Description = "Hora inicio OT", SubType = SubTipoCampo.Time, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "USEROT", Type = TipoCampo.Alpha, Size = 25, Description = "Usuario inicio OT", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINENUMOV", Type = TipoCampo.Numeric, Size = 11, Description = "Línea detalle OV", SubType = SubTipoCampo.None, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTSOLICITADA", Type = TipoCampo.Float, Size = 10, Description = "Cantidad solicitada", SubType = SubTipoCampo.Quantity, TableName = "@EEP_PLANI_OT", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // OT Inyección - Extrusión
                new () { Name = "FECHAOT", Type = TipoCampo.Date, Size = 10, Description = "Fecha OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODARTICULO", Type = TipoCampo.Alpha, Size = 50, Description = "Código artículo OV", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODSUBART", Type = TipoCampo.Alpha, Size = 50, Description = "Cod. sub artículo", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTIDADOT", Type = TipoCampo.Float, Size = 10, Description = "Cantidad OT", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DOCENTRYOV", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry OV", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTANTERIOR", Type = TipoCampo.Alpha, Size = 50, Description = "Estación anterior", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODEPLANIOT", Type = TipoCampo.Numeric, Size = 11, Description = "Code planificación OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "USEROT", Type = TipoCampo.Alpha, Size = 25, Description = "Usuario creador OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTACION", Type = TipoCampo.Alpha, Size = 50, Description = "Estación actual", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                // OT Inyección - Extrusión Det.
                new () { Name = "NROCONTEN", Type = TipoCampo.Alpha, Size = 50, Description = "Nro. contenedor", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "NROMAQUI", Type = TipoCampo.Alpha, Size = 25, Description = "Nro. máquina", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAPROC", Type = TipoCampo.Date, Size = 10, Description = "Fecha", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAINI", Type = TipoCampo.Date, Size = 8, Description = "Hora inicio trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAFIN", Type = TipoCampo.Date, Size = 8, Description = "Hora fin trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TURNO", Type = TipoCampo.Alpha, Size = 2, Description = "Turno operador", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO", Type = TipoCampo.Alpha, Size = 25, Description = "Operario", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTAPROB", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades aprobadas", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTRET", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades retenidas", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTMERMA", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades rechazado reciclable", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTMERMAKG", Type = TipoCampo.Float, Size = 10, Description = "Peso rechazado reciclable (KG)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "MOTIVOMERMA", Type = TipoCampo.Alpha, Size = 100, Description = "Motivo merma", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTMERMA2", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades rechazado no reciclable", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTMERMAKG2", Type = TipoCampo.Float, Size = 10, Description = "Peso rechazado no reciclable (KG)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "MOTIVOMERMA2", Type = TipoCampo.Alpha, Size = 100, Description = "Motivo merma 2", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP1", Type = TipoCampo.Float, Size = 10, Description = "Peso colada (KG)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP2", Type = TipoCampo.Float, Size = 10, Description = "Peso masacote (KG)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP3", Type = TipoCampo.Float, Size = 10, Description = "Peso por ajustes de máq. (KG)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP4", Type = TipoCampo.Float, Size = 10, Description = "Peso pieza (G)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP5", Type = TipoCampo.Numeric, Size = 11, Description = "Cavidades reales", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP6", Type = TipoCampo.Numeric, Size = 11, Description = "Cavidades operativas", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP7", Type = TipoCampo.Float, Size = 10, Description = "Tiempo de ciclo real (S)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP8", Type = TipoCampo.Float, Size = 10, Description = "Tiempo de ciclo (S)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LIBERADO", Type = TipoCampo.Alpha, Size = 1, Description = "Liberación de producción", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DEENTRADA", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry entrada", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DESALIDA", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry salida", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado línea", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "NROASIENTO", Type = TipoCampo.Numeric, Size = 11, Description = "Número asiento", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP9", Type = TipoCampo.Float, Size = 10, Description = "Metros por minuto real", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP10", Type = TipoCampo.Float, Size = 10, Description = "Metros por minuto", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP11", Type = TipoCampo.Float, Size = 10, Description = "Metros por turno real", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP12", Type = TipoCampo.Float, Size = 10, Description = "Metros por turno", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO2", Type = TipoCampo.Alpha, Size = 25, Description = "Operario 2", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LOTE", Type = TipoCampo.Alpha, Size = 25, Description = "Lote", SubType = SubTipoCampo.None, TableName = "@EEP_OT_INYEX_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // Registro paradas
                new () { Name = "ESTACION", Type = TipoCampo.Alpha, Size = 50, Description = "Estación de trabajo", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OT", Type = TipoCampo.Numeric, Size = 11, Description = "Nro. OT", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINEIDOT", Type = TipoCampo.Numeric, Size = 11, Description = "LineId OT", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHA", Type = TipoCampo.Date, Size = 10, Description = "Fecha registro", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TIPOPARO", Type = TipoCampo.Alpha, Size = 50, Description = "Tipo parada", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TURNO", Type = TipoCampo.Alpha, Size = 2, Description = "Turno", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERADOR1", Type = TipoCampo.Alpha, Size = 25, Description = "Operador 1", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERADOR2", Type = TipoCampo.Alpha, Size = 25, Description = "Operador 2", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAINI", Type = TipoCampo.Date, Size = 8, Description = "Hora inicio parada", SubType = SubTipoCampo.Time, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAFIN", Type = TipoCampo.Date, Size = 8, Description = "Hora fin parada", SubType = SubTipoCampo.Time, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "NROMAQUI", Type = TipoCampo.Alpha, Size = 25, Description = "Nro. máquina", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado parada", SubType = SubTipoCampo.None, TableName = "@EEP_PARADAS", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // Planificación ensamble
                new () { Name = "CODEPLANIOT", Type = TipoCampo.Numeric, Size = 11, Description = "Code planificación OT", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTANTERIOR", Type = TipoCampo.Alpha, Size = 50, Description = "Estación anterior", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                // Planificación ensamble - Detalle
                new () { Name = "CANTPROD", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad producida", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTPRODKG", Type = TipoCampo.Float, Size = 10, Description = "Cantidad producida KG", SubType = SubTipoCampo.Quantity, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTSOLICITADA", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad solicitada", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OT", Type = TipoCampo.Numeric, Size = 11, Description = "Nro. OT estación anterior", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINEIDOT", Type = TipoCampo.Numeric, Size = 11, Description = "LineId estación anterior", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODSUBART", Type = TipoCampo.Alpha, Size = 50, Description = "Cod. sub artículo", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado OT", SubType = SubTipoCampo.None, TableName = "@EEP_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // OT Ensamble
                new () { Name = "CODEPLANIOT", Type = TipoCampo.Numeric, Size = 11, Description = "Code planificación OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTACION", Type = TipoCampo.Alpha, Size = 50, Description = "Estación de trabajo", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAOT", Type = TipoCampo.Date, Size = 10, Description = "Fecha OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_CAB", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "USEROT", Type = TipoCampo.Alpha, Size = 25, Description = "Usuario creador OT", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CODSUBART", Type = TipoCampo.Alpha, Size = 50, Description = "Cod. sub artículo", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_CAB", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                // OT Ensamble Det.
                new () { Name = "NROCONTEN", Type = TipoCampo.Alpha, Size = 50, Description = "Nro. contenedor", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "NROMAQUI", Type = TipoCampo.Alpha, Size = 25, Description = "Nro. máquina", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAPROC", Type = TipoCampo.Date, Size = 10, Description = "Fecha", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAINI", Type = TipoCampo.Date, Size = 8, Description = "Hora inicio trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAFIN", Type = TipoCampo.Date, Size = 8, Description = "Hora fin trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TURNO", Type = TipoCampo.Alpha, Size = 2, Description = "Turno operador", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO", Type = TipoCampo.Alpha, Size = 25, Description = "Operario", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO2", Type = TipoCampo.Alpha, Size = 25, Description = "Operario 2", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTAPROB", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades aprobadas", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTPRODKG", Type = TipoCampo.Float, Size = 10, Description = "Cantidad aprobadas KG", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CCP1", Type = TipoCampo.Float, Size = 10, Description = "Peso pieza (G)", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = "0" , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTAPROBD", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidades aprobadas desvio", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OBS", Type = TipoCampo.Alpha, Size = 100, Description = "Observaciones", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LIBERADO", Type = TipoCampo.Alpha, Size = 1, Description = "Liberación de producción", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DEENTRADA", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry entrada", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DESALIDA", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry salida", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTADO", Type = TipoCampo.Alpha, Size = 50, Description = "Estado línea", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "NROASIENTO", Type = TipoCampo.Numeric, Size = 11, Description = "Número asiento", SubType = SubTipoCampo.None, TableName = "@EEP_OT_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                // OT Registro Ensamble Det.
                new () { Name = "CODSUBART", Type = TipoCampo.Alpha, Size = 50, Description = "Cod. sub artículo", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LOTE", Type = TipoCampo.Alpha, Size = 25, Description = "Lote", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTIDAD", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TIPO", Type = TipoCampo.Alpha, Size = 20, Description = "Tipo registro", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTREP", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad reproceso", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO", Type = TipoCampo.Alpha, Size = 25, Description = "Operario", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO2", Type = TipoCampo.Alpha, Size = 25, Description = "Operario 2", SubType = SubTipoCampo.None, TableName = "@EEP_REG_ENSAM_DET", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // OT Pendiente a molinar
                new () { Name = "CODEPLANIOT", Type = TipoCampo.Numeric, Size = 11, Description = "Code planificación OT", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "ESTANTERIOR", Type = TipoCampo.Alpha, Size = 50, Description = "Estación anterior", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OT", Type = TipoCampo.Numeric, Size = 11, Description = "Nro. OT", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINEIDOT", Type = TipoCampo.Numeric, Size = 11, Description = "LineId OT", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LOTE", Type = TipoCampo.Alpha, Size = 25, Description = "Lote", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTPROD", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad producida", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTSOLICITADA", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad solicitada", SubType = SubTipoCampo.None, TableName = "@EEP_PEND_MOLI_OT", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // OT Molino
                new () { Name = "DEPENDMOLI", Type = TipoCampo.Numeric, Size = 11, Description = "DocEntry pendiente a molinar", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "FECHAPROC", Type = TipoCampo.Date, Size = 0, Description = "Fecha", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAINI", Type = TipoCampo.Date, Size = 0, Description = "Hora inicio trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "HORAFIN", Type = TipoCampo.Date, Size = 0, Description = "Hora fin trabajo", SubType = SubTipoCampo.Time, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TURNO", Type = TipoCampo.Alpha, Size = 2, Description = "Turno operador", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO", Type = TipoCampo.Alpha, Size = 25, Description = "Operario", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OPERARIO2", Type = TipoCampo.Alpha, Size = 25, Description = "Operario 2", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTPROC", Type = TipoCampo.Numeric, Size = 11, Description = "Cantidad a procesar", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTRECIKG", Type = TipoCampo.Float, Size = 10, Description = "Cantidad reciclable KG", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CANTRECHKG", Type = TipoCampo.Float, Size = 10, Description = "Cantidad no conforme KG", SubType = SubTipoCampo.Quantity, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "MOTIVORECH", Type = TipoCampo.Alpha, Size = 100, Description = "Motivo rechazo", SubType = SubTipoCampo.None, TableName = "@EEP_OT_MOLINO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

                // Maestro paradas/motivos defectos
                new () { Name = "CODIGO", Type = TipoCampo.Alpha, Size = 3, Description = "Código mot. parada/defecto", SubType = SubTipoCampo.None, TableName = "@EEP_PARADA_DEFECTO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "DESCRIPCION", Type = TipoCampo.Alpha, Size = 50, Description = "Descripción mot. parada/defecto", SubType = SubTipoCampo.None, TableName = "@EEP_PARADA_DEFECTO", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "TIPO", Type = TipoCampo.Alpha, Size = 2, Description = "Tipo", SubType = SubTipoCampo.None, TableName = "@EEP_PARADA_DEFECTO", LinkedTable = null, DefaultValue = "PR",
                    ValidValuesMD =
                    [
                        new ValidValuesMD() { Value = "PR", Description = "Parada"},
                        new ValidValuesMD() { Value = "DF", Description = "Defecto"}
                    ],
                    LinkedSystemObject = null },

                // Campos SAP
                new () { Name = "OTREFE", Type = TipoCampo.Numeric, Size = 11, Description = "OT referencial", SubType = SubTipoCampo.None, TableName = "OIGE", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINEREFE", Type = TipoCampo.Numeric, Size = 11, Description = "LineId referencial", SubType = SubTipoCampo.None, TableName = "OIGE", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "OTREFE", Type = TipoCampo.Numeric, Size = 11, Description = "OT referencial", SubType = SubTipoCampo.None, TableName = "OIGN", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "LINEREFE", Type = TipoCampo.Numeric, Size = 11, Description = "LineId referencial", SubType = SubTipoCampo.None, TableName = "OIGN", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "CodAcabado", Type = TipoCampo.Alpha, Size = 50, Description = "Subproducto", SubType = SubTipoCampo.None, TableName = "ITT2", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "MOLIRE", Type = TipoCampo.Alpha, Size = 50, Description = "Art. molinado re.", SubType = SubTipoCampo.None, TableName = "OITM", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
                new () { Name = "MOLINORE", Type = TipoCampo.Alpha, Size = 50, Description = "Art. molinado no re.", SubType = SubTipoCampo.None, TableName = "OITM", LinkedTable = null, DefaultValue = null , ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
            }.ForEach(AgregarCampos);
            #endregion

            #region Crear UDOS
            new List<UserObjectsMD>
            {
                new ()
                {
                    Code = "EEP_PERM", Name = "EEP Maestro Permisos", TableName = "EEP_PERM", ObjectType = TipoObjeto.DatoMaestro, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tYES, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_PERM", ColumnAlias = "Code", ColumnDescription = "Código" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_PERM", ColumnAlias = "Name", ColumnDescription = "Nombre" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_PERM", ColumnAlias = "U_DESC", ColumnDescription = "Descripción" }
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_PERM", FormColumnAlias = "Code", FormColumnDescription = "Código" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_PERM", FormColumnAlias = "Name", FormColumnDescription = "Nombre" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_PERM", FormColumnAlias = "U_DESC", FormColumnDescription = "Descripción" }
                    ]
                },
                new ()
                {
                    Code = "EEP_ROLU", Name = "EEP RolesUsuarios", TableName = "EEP_ROLU", ObjectType = TipoObjeto.DatoMaestro, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tYES, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLU", ColumnAlias = "Code", ColumnDescription = "Código" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLU", ColumnAlias = "Name", ColumnDescription = "Descripción" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLU", ColumnAlias = "U_USER", ColumnDescription = "Usuario" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLU", ColumnAlias = "U_ROLID", ColumnDescription = "Rol" }
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLU", FormColumnAlias = "Code", FormColumnDescription = "Código" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLU", FormColumnAlias = "Name", FormColumnDescription = "Descripción" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLU", FormColumnAlias = "U_USER", FormColumnDescription = "Usuario" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLU", FormColumnAlias = "U_ROLID", FormColumnDescription = "Rol" }
                    ]
                },
                new ()
                {
                    Code = "EEP_ROLC", Name = "EEP Maestro Roles", TableName = "EEP_ROLC", ObjectType = TipoObjeto.DatoMaestro, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tYES, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO,
                    ChildTables =
                    [
                        new UserChildTablesMD() { Code = "EEP_ROLC", SonNumber = "1", TableName = "EEP_ROLD", ObjectName = "EEP_ROLD" }
                    ],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLC", ColumnAlias = "Code", ColumnDescription = "Código Rol" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLC", ColumnAlias = "Name", ColumnDescription = "Nombre Rol" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_ROLC", ColumnAlias = "U_ACTI", ColumnDescription = "Activo" }
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLC", FormColumnAlias = "Code", FormColumnDescription = "Código Rol" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLC", FormColumnAlias = "Name", FormColumnDescription = "Nombre Rol" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_ROLC", FormColumnAlias = "U_ACTI", FormColumnDescription = "Activo" }
                    ]
                },
                new ()
                {
                    Code = "EEP_PARAM", Name = "EEP Parametrización", TableName = "EEP_PARAM", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO,
                    ChildTables =
                    [
                        new UserChildTablesMD() { Code = "EEP_PARAM", SonNumber = "1", TableName = "EEP_PARSERIE_DET", ObjectName = "EEP_PARSERIE_DET" }
                    ],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARAM", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARAM", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                new ()
                {
                    Code = "EEP_OT_INYEX_CAB", Name = "EEP OT Inyección/Extrusión", TableName = "EEP_OT_INYEX_CAB", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO,
                    ChildTables =
                    [
                        new UserChildTablesMD() { Code = "EEP_OT_INYEX_CAB", SonNumber = "1", TableName = "EEP_OT_INYEX_DET", ObjectName = "EEP_OT_INYEX_DET" }
                    ],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_OT_INYEX_CAB", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_OT_INYEX_CAB", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                new ()
                {
                    Code = "EEP_PARADAS", Name = "EEP Registro paradas", TableName = "EEP_PARADAS", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARADAS", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARADAS", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                new ()
                {
                    Code = "EEP_PARADA_DEFECTO", Name = "EEP_PARADA_DEFECTO", TableName = "EEP_PARADA_DEFECTO", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tYES, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARADA_DEFECTO", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARADA_DEFECTO", ColumnAlias = "U_CODIGO", ColumnDescription = "Código" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARADA_DEFECTO", ColumnAlias = "U_DESCRIPCION", ColumnDescription = "Descripción" },
                        new UserColumnsMD_FindColumns() { Code = "EEP_PARADA_DEFECTO", ColumnAlias = "U_TIPO", ColumnDescription = "Tipo" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARADA_DEFECTO", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARADA_DEFECTO", FormColumnAlias = "U_CODIGO", FormColumnDescription = "Código" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARADA_DEFECTO", FormColumnAlias = "U_DESCRIPCION", FormColumnDescription = "Descripción" },
                        new UserColumnsMD_FormColumns() { Code = "EEP_PARADA_DEFECTO", FormColumnAlias = "U_TIPO", FormColumnDescription = "Tipo" },
                    ]
                },
                new ()
                {
                    Code = "EEP_ENSAM_CAB", Name = "EEP Planificación ensamblado", TableName = "EEP_ENSAM_CAB", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO,
                    ChildTables =
                    [
                        new UserChildTablesMD() { Code = "EEP_ENSAM_CAB", SonNumber = "1", TableName = "EEP_ENSAM_DET", ObjectName = "EEP_ENSAM_DET" }
                    ],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_ENSAM_CAB", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_ENSAM_CAB", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                new ()
                {
                    Code = "EEP_OT_ENSAM_CAB", Name = "EEP OT Ensamblado", TableName = "EEP_OT_ENSAM_CAB", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tNO, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO,
                    ChildTables =
                    [
                        new UserChildTablesMD() { Code = "EEP_OT_ENSAM_CAB", SonNumber = "1", TableName = "EEP_OT_ENSAM_DET", ObjectName = "EEP_OT_ENSAM_DET" },
                        new UserChildTablesMD() { Code = "EEP_OT_ENSAM_CAB", SonNumber = "2", TableName = "EEP_REG_ENSAM_DET", ObjectName = "EEP_REG_ENSAM_DET" }
                    ],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_OT_ENSAM_CAB", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_OT_ENSAM_CAB", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                 new ()
                {
                    Code = "EEP_PEND_MOLI_OT", Name = "EEP OT Pendiente a molinar", TableName = "EEP_PEND_MOLI_OT", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tYES, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_PEND_MOLI_OT", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_PEND_MOLI_OT", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
                  new ()
                {
                    Code = "EEP_OT_MOLINO", Name = "EEP OT Molino", TableName = "EEP_OT_MOLINO", ObjectType = TipoObjeto.Documento, CanFind = BoYesNo.tYES, CanCancel = BoYesNo.tNO, CanDelete = BoYesNo.tYES,
                    CanLog = BoYesNo.tYES, CanCreateDefaultForm = BoYesNo.tYES, EnableEnhancedForm = BoYesNo.tNO, RebuildEnhancedForm = BoYesNo.tNO, ChildTables = [],
                    FindColumns =
                    [
                        new UserColumnsMD_FindColumns() { Code = "EEP_OT_MOLINO", ColumnAlias = "DocEntry", ColumnDescription = "DocEntry" },
                    ],
                    FormColumns =
                    [
                        new UserColumnsMD_FormColumns() { Code = "EEP_OT_MOLINO", FormColumnAlias = "DocEntry", FormColumnDescription = "DocEntry" },
                    ]
                },
            }.ForEach(AgregarObjetos);
            #endregion

            #region Agregar registros
            new List<MasterData>
            {
                // Permisos
                new () { TableName = "EEP_PERM", Code = "01", Name = "Menú Gestión de Accesos", Descripcion = "Acceso a la gestión de Usuarios, Roles y Permisos" },
                new () { TableName = "EEP_PERM", Code = "02", Name = "Generar Estructura", Descripcion = "Generación de estructura del sistema" },
                new () { TableName = "EEP_PERM", Code = "04", Name = "Parametrización", Descripcion = "Parametrización del sistema" },

                // Roles
                new () { TableName = "EEP_ROLC", Code = "01", Name = "Administrador", Activo = YesNo.Yes },
                new () { TableName = "EEP_ROLC", Code = "02", Name = "Usuario", Activo = YesNo.Yes },
            }.ForEach(AgregarDatosMaestros);
            #endregion

            #region Crear SP
            new List<UserQuerysMD>
            {
                new () { Procedure = "SP_INSERT_PLANIFICACION_OT" },
            }.ForEach(AgregarProcedimientos);
            #endregion
        }

        public void AgregarTablas(UserTablesMD userTablesMD)
        {
            var che = ObtenerTabla(userTablesMD.TableName);

            if (che.TableName == null)
            {
                var method = Method.Post;
                var entity = $"UserTablesMD";

                var body = new
                {
                    TableName = userTablesMD.TableName,
                    TableDescription = userTablesMD.Descr,
                    TableType = userTablesMD.ObjectType
                };

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public void AgregarCampos(UserFieldsMD userFieldsMD)
        {
            var list = ObtenerCampo(userFieldsMD.TableName, userFieldsMD.Name);

            if (list.Count == 0)
            {
                var method = Method.Post;
                var entity = $"UserFieldsMD";

                var body = new
                {
                    Name = userFieldsMD.Name,
                    Type = userFieldsMD.Type,
                    Size = userFieldsMD.Size,
                    EditSize = userFieldsMD.Size,
                    Description = userFieldsMD.Description,
                    SubType = userFieldsMD.SubType,
                    TableName = userFieldsMD.TableName,
                    LinkedTable = userFieldsMD.LinkedTable,
                    DefaultValue = userFieldsMD.DefaultValue,
                    ValidValuesMD = userFieldsMD.ValidValuesMD,
                    LinkedSystemObject = userFieldsMD.LinkedSystemObject
                };

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public void AgregarObjetos(UserObjectsMD userObjectsMD)
        {
            var che = ObtenerObjeto(userObjectsMD.TableName);

            if (che.Code == null)
            {
                var method = Method.Post;
                var entity = $"UserObjectsMD";

                var body = new
                {
                    Code = userObjectsMD.Code,
                    Name = userObjectsMD.Name,
                    TableName = userObjectsMD.TableName,
                    ObjectType = userObjectsMD.ObjectType,
                    CanFind = userObjectsMD.CanFind,
                    CanCancel = userObjectsMD.CanCancel,
                    CanDelete = userObjectsMD.CanDelete,
                    CanLog = userObjectsMD.CanLog,
                    CanCreateDefaultForm = userObjectsMD.CanCreateDefaultForm,
                    EnableEnhancedForm = userObjectsMD.EnableEnhancedForm,
                    RebuildEnhancedForm = userObjectsMD.RebuildEnhancedForm,
                    UserObjectMD_ChildTables = userObjectsMD.ChildTables,
                    UserObjectMD_FindColumns = userObjectsMD.FindColumns,
                    UserObjectMD_FormColumns = userObjectsMD.FormColumns
                };

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public void AgregarDatosMaestros(MasterData masterData)
        {
            var list = ObtenerDatoMaestro(masterData.TableName, masterData.Code);

            if (list.Count == 0)
            {
                var method = Method.Post;
                var entity = $"{masterData.TableName}";
                dynamic body = null;

                switch (masterData.TableName)
                {
                    case "EEP_PERM":
                        body = new
                        {
                            Code = masterData.Code,
                            Name = masterData.Name,
                            U_DESC = masterData.Descripcion
                        };
                        break;

                    case "EEP_ROLC":
                        body = new
                        {
                            Code = masterData.Code,
                            Name = masterData.Name,
                            U_ACTI = masterData.Activo
                        };
                        break;
                }

                var jObject = _connectionService.SetEntitySL(method, entity, body);
            }
        }

        public void AgregarProcedimientos(UserQuerysMD UserQuerysMD)
        {
            var existe = ObtenerProcedimiento(UserQuerysMD.Procedure);

            if (!existe)
            {
                CrearProcedimiento(UserQuerysMD.Procedure);
            }
        }

        public UserTablesMD ObtenerTabla(string tabla)
        {
            var che = new UserTablesMD();

            var query = "SELECT \"TableName\", \"TblNum\", \"Descr\", \"ObjectType\"  " +
                    $"FROM \"{_connectionService.DataBase}\".\"OUTB\" " +
                    $"WHERE \"TableName\" = '{tabla}'";


            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                che.TableName = reader["TableName"].ToString();
                che.TblNum = (int)reader["TblNum"];
                che.Descr = reader["Descr"].ToString();
                che.ObjectType = int.Parse(reader["ObjectType"].ToString());
            }

            _connectionService.DisconnectODBC();

            return che;
        }

        public List<UserFieldsMD> ObtenerCampo(string tabla, string alias)
        {
            var list = new List<UserFieldsMD>();

            var query = "SELECT \"AliasID\", \"FieldID\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"CUFD\" " +
                    $"WHERE \"TableID\" = '{tabla}'" +
                    $"AND \"AliasID\" = '{alias}'";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new UserFieldsMD();
                che.Name = reader["AliasID"].ToString();
                che.FieldID = (short)reader["FieldID"];
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public UserObjectsMD ObtenerObjeto(string tabla)
        {
            var che = new UserObjectsMD();

            var query = "SELECT \"Code\", \"TableName\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"OUDO\" " +
                    $"WHERE \"TableName\" = '{tabla}'";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                che.Code = reader["Code"].ToString();
                che.TableName = reader["TableName"].ToString();
            }

            _connectionService.DisconnectODBC();

            return che;
        }

        public List<MasterData> ObtenerDatoMaestro(string tabla, string codigo)
        {
            var list = new List<MasterData>();

            var query = "SELECT \"Code\", \"Name\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"@{tabla}\" " +
                    $"WHERE \"Code\" = '{codigo}'";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new MasterData();
                che.Code = reader["Code"].ToString();
                che.Name = reader["Name"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public bool ObtenerProcedimiento(string procedimiento)
        {
            bool valido = true;

            var query = "SELECT CAST(COUNT(*) AS INTEGER) AS \"CANTIDAD\" " +
                    $"FROM SYS.PROCEDURES " +
                    $"WHERE \"SCHEMA_NAME\" = '{_connectionService.DataBase}' " +
                    $"AND \"PROCEDURE_NAME\" = '{procedimiento}'";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                valido = (int)reader["CANTIDAD"] == 1;
            }

            return valido;
        }

        public void CrearProcedimiento(string procedimiento)
        {
            var query = $"SET SCHEMA \"{_connectionService.DataBase}\"";

            var command = new OdbcCommand("", _connectionService.ConnectODBC());
            command.CommandText = query;
            command.ExecuteNonQuery();

            var sqlFile = $"{Path.Combine(Directory.GetCurrentDirectory())}\\Installer\\HANA\\{procedimiento}.sql";
            var sqlText = System.IO.File.ReadAllText(sqlFile);

            command.CommandText = sqlText;
            command.ExecuteNonQuery();

            _connectionService.DisconnectODBC();
        }
    }
}

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
    }.ForEach(AgregarTablas);
            #endregion

            #region Crear campos
            var valoresValidosVacios = new List<ValidValuesMD>(); // Lista vacía
            new List<UserFieldsMD>
    {
        // Campos para EEP_PERM
        new () { Name = "DESC", Type = TipoCampo.Alpha, Size = 100, Description = "Descripción", SubType = SubTipoCampo.None, TableName = "@EEP_PERM", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

        // Campos para EEP_ROLU
        new () { Name = "USER", Type = TipoCampo.Alpha, Size = 25, Description = "Usuario", SubType = SubTipoCampo.None, TableName = "@EEP_ROLU", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
        new () { Name = "ROLID", Type = TipoCampo.Alpha, Size = 10, Description = "Rol", SubType = SubTipoCampo.None, TableName = "@EEP_ROLU", LinkedTable = "EEP_ROLC", DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

        // Campos para EEP_ROLC
        new () { Name = "ACTI", Type = TipoCampo.Alpha, Size = 1, Description = "Activo", SubType = SubTipoCampo.None, TableName = "@EEP_ROLC", LinkedTable = null, DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },

        // Campos para EEP_ROLD (hija de EEP_ROLC)
        new () { Name = "PERM", Type = TipoCampo.Alpha, Size = 1, Description = "Código de Permiso", SubType = SubTipoCampo.None, TableName = "@EEP_ROLD", LinkedTable = "EEP_PERM", DefaultValue = null, ValidValuesMD = valoresValidosVacios, LinkedSystemObject = null },
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
    }.ForEach(AgregarObjetos);
            #endregion

            #region Agregar registros
            new List<MasterData>
    {
        // Permisos (ejemplos)
        new () { TableName = "EEP_PERM", Code = "01", Name = "Menú Gestión de Accesos", Descripcion = "Acceso a la gestión de Usuarios, Roles y Permisos" },
        new () { TableName = "EEP_PERM", Code = "02", Name = "Generar Estructura", Descripcion = "Generación de estructura del sistema" },
        new () { TableName = "EEP_PERM", Code = "04", Name = "Parametrización", Descripcion = "Parametrización del sistema" },
        new () { TableName = "EEP_PERM", Code = "05", Name = "Lotes / Inmueble", Descripcion = "Acceso a la gestión de lotes e inmuebles" },
        new () { TableName = "EEP_PERM", Code = "06", Name = "Alta de clientes y proveedores", Descripcion = "Acceso al alta de clientes y proveedores" },
        new () { TableName = "EEP_PERM", Code = "07", Name = "Seña de lote", Descripcion = "Acceso al registro de señas de lote" },
        new () { TableName = "EEP_PERM", Code = "08", Name = "Generación de contrato", Descripcion = "Acceso a la generación de contratos" },
        new () { TableName = "EEP_PERM", Code = "09", Name = "Comisiones a vendedores", Descripcion = "Acceso a la gestión de comisiones de vendedores" },
        new () { TableName = "EEP_PERM", Code = "10", Name = "Planes de comisión a vendedores", Descripcion = "Acceso a la gestión de planes de comisión" },
        new () { TableName = "EEP_PERM", Code = "11", Name = "Planes de ventas", Descripcion = "Acceso a la gestión de planes de ventas" },
        new () { TableName = "EEP_PERM", Code = "12", Name = "Consulta de estado de contratos", Descripcion = "Consulta del estado de contratos" },
        new () { TableName = "EEP_PERM", Code = "13", Name = "Listado de lotes libres", Descripcion = "Consulta de lotes disponibles" },
        new () { TableName = "EEP_PERM", Code = "14", Name = "Ventas realizadas", Descripcion = "Consulta de ventas realizadas" },
        new () { TableName = "EEP_PERM", Code = "15", Name = "Reporte de recupero y renuncias", Descripcion = "Consulta de recuperos y renuncias" },
        new () { TableName = "EEP_PERM", Code = "16", Name = "Reporte para Sepralad", Descripcion = "Acceso al reporte para Sepralad" },
        new () { TableName = "EEP_PERM", Code = "17", Name = "Redefinir cuotas de contrato", Descripcion = "Permite redefinir cuotas de contratos" },
        new () { TableName = "EEP_PERM", Code = "18", Name = "Asignación de operadores a fracción", Descripcion = "Acceso a la asignación de operadores a fracciones" },
        new () { TableName = "EEP_PERM", Code = "19", Name = "Liquidación de propietario", Descripcion = "Acceso a la liquidación de propietarios" },
        new () { TableName = "EEP_PERM", Code = "20", Name = "Consulta de liquidación por propietario", Descripcion = "Consulta de liquidaciones por propietario" },
        new () { TableName = "EEP_PERM", Code = "21", Name = "Reporte de rendición de propietarios", Descripcion = "Consulta de rendiciones de propietarios" },
        new () { TableName = "EEP_PERM", Code = "22", Name = "Reporte de liquidación de propietarios", Descripcion = "Consulta de liquidaciones de propietarios" },
        new () { TableName = "EEP_PERM", Code = "23", Name = "Consulta de facturas", Descripcion = "Consulta de facturas emitidas" },
        new () { TableName = "EEP_PERM", Code = "24", Name = "Loteamiento de fracciones", Descripcion = "Acceso a la gestión de loteamiento de fracciones" },
        new () { TableName = "EEP_PERM", Code = "25", Name = "Liquidación de cuotas", Descripcion = "Acceso a la liquidación de cuotas" },
        new () { TableName = "EEP_PERM", Code = "26", Name = "Generación de planilla de reclamos", Descripcion = "Acceso a la generación de planillas de reclamos" },
        new () { TableName = "EEP_PERM", Code = "27", Name = "Historial de llamadores", Descripcion = "Consulta del historial de llamadores" },
        new () { TableName = "EEP_PERM", Code = "28", Name = "Gestión de reclamos", Descripcion = "Acceso a la gestión de reclamos" },
        new () { TableName = "EEP_PERM", Code = "29", Name = "Extracto de contrato", Descripcion = "Consulta de extractos de contrato" },
        new () { TableName = "EEP_PERM", Code = "30", Name = "Renuncia a lote", Descripcion = "Acceso al proceso de renuncia de lotes" },
        new () { TableName = "EEP_PERM", Code = "31", Name = "Solicitud de recupero de lote", Descripcion = "Acceso a solicitudes de recupero de lotes" },
        new () { TableName = "EEP_PERM", Code = "32", Name = "Cuotas cobradas por fracción", Descripcion = "Consulta de cuotas cobradas por fracción" },
        new () { TableName = "EEP_PERM", Code = "33", Name = "Cartera de clientes", Descripcion = "Consulta de cartera de clientes" },
        new () { TableName = "EEP_PERM", Code = "34", Name = "Mantenimiento de verificación de lotes", Descripcion = "Acceso al mantenimiento de verificación de lotes" },
        new () { TableName = "EEP_PERM", Code = "35", Name = "Solicitud de bloqueo de contrato", Descripcion = "Acceso a solicitudes de bloqueo de contrato" },
        new () { TableName = "EEP_PERM", Code = "36", Name = "Bloqueos masivos", Descripcion = "Acceso a bloqueos masivos de contratos" },
        new () { TableName = "EEP_PERM", Code = "37", Name = "Pedido de descuento", Descripcion = "Acceso a pedidos de descuento" },
        new () { TableName = "EEP_PERM", Code = "38", Name = "Datos de clientes", Descripcion = "Consulta y mantenimiento de datos de clientes" },
        new () { TableName = "EEP_PERM", Code = "39", Name = "Archivo Inforcom", Descripcion = "Acceso a la gestión y consulta de archivos Inforcom" },
        new () { TableName = "EEP_PERM", Code = "40", Name = "Cobro Ventanilla", Descripcion = "Acceso al cobro por ventanilla" },
        new () { TableName = "EEP_PERM", Code = "41", Name = "Cobros múltiples", Descripcion = "Acceso a cobros múltiples" },
        new () { TableName = "EEP_PERM", Code = "42", Name = "Cobro cuota escrituración", Descripcion = "Acceso al cobro de cuotas de escrituración" },
        new () { TableName = "EEP_PERM", Code = "43", Name = "Arqueo de CAJA", Descripcion = "Acceso al arqueo de caja" },
        new () { TableName = "EEP_PERM", Code = "44", Name = "Registro de servicio a facturar", Descripcion = "Acceso al registro de servicios a facturar" },
        new () { TableName = "EEP_PERM", Code = "45", Name = "Registro de pago impuesto inmobiliario", Descripcion = "Acceso al registro de pago de impuesto inmobiliario" },
        new () { TableName = "EEP_PERM", Code = "46", Name = "Estado de documentos facturación electrónica", Descripcion = "Consulta del estado de documentos de facturación electrónica" },
        new () { TableName = "EEP_PERM", Code = "47", Name = "Reimpresión de facturas y recibos", Descripcion = "Acceso a la reimpresión de facturas y recibos" },
        new () { TableName = "EEP_PERM", Code = "48", Name = "Devolución de venta (nota de crédito)", Descripcion = "Acceso a devoluciones de venta mediante nota de crédito" },
        new () { TableName = "EEP_PERM", Code = "49", Name = "Mantenimiento de cotización", Descripcion = "Acceso al mantenimiento de cotizaciones" },
        new () { TableName = "EEP_PERM", Code = "50", Name = "Libro banco", Descripcion = "Consulta del libro banco" },
        new () { TableName = "EEP_PERM", Code = "51", Name = "Orden de pago", Descripcion = "Acceso a órdenes de pago" },
        new () { TableName = "EEP_PERM", Code = "52", Name = "Anulación de documentos", Descripcion = "Acceso a la anulación de documentos" },
        new () { TableName = "EEP_PERM", Code = "53", Name = "Consulta de documentos anulados", Descripcion = "Consulta de documentos anulados" },
        new () { TableName = "EEP_PERM", Code = "54", Name = "Movimientos varios", Descripcion = "Acceso a movimientos varios" },
        new () { TableName = "EEP_PERM", Code = "55", Name = "Operador cuenta banco", Descripcion = "Acceso a la gestión de operadores de cuentas bancarias" },
        new () { TableName = "EEP_PERM", Code = "56", Name = "Mantenimiento de cuentas bancarias", Descripcion = "Acceso al mantenimiento de cuentas bancarias" },
        new () { TableName = "EEP_PERM", Code = "57", Name = "Mantenimiento proveedores casuales", Descripcion = "Acceso al mantenimiento de proveedores casuales" },
        new () { TableName = "EEP_PERM", Code = "58", Name = "Órdenes de pagos banco familiar", Descripcion = "Acceso a órdenes de pago del Banco Familiar" },
        new () { TableName = "EEP_PERM", Code = "59", Name = "Extracto de cuentas de proveedores", Descripcion = "Consulta de extractos de cuentas de proveedores" },
        new () { TableName = "EEP_PERM", Code = "60", Name = "Depósito/extracción", Descripcion = "Acceso a operaciones de depósito y extracción" },
        new () { TableName = "EEP_PERM", Code = "61", Name = "Pagos/cobros", Descripcion = "Acceso a pagos y cobros" },
        new () { TableName = "EEP_PERM", Code = "62", Name = "Cobros desde bocas de cobranzas", Descripcion = "Acceso a cobros desde bocas de cobranza" },
        new () { TableName = "EEP_PERM", Code = "63", Name = "Cesión de derecho", Descripcion = "Acceso al proceso de cesión de derecho" },
        new () { TableName = "EEP_PERM", Code = "64", Name = "Orden de escrituración/plan de pago", Descripcion = "Acceso a órdenes de escrituración y planes de pago" },
        new () { TableName = "EEP_PERM", Code = "65", Name = "Consulta cuotas de escrituración", Descripcion = "Consulta de cuotas de escrituración" },
        new () { TableName = "EEP_PERM", Code = "66", Name = "Detalles por centro de costo", Descripcion = "Consulta de detalles por centro de costo" },
        new () { TableName = "EEP_PERM", Code = "67", Name = "Libro diario de contabilidad", Descripcion = "Consulta del libro diario de contabilidad" },
        new () { TableName = "EEP_PERM", Code = "68", Name = "Sub diario de control", Descripcion = "Consulta del sub diario de control" },
        new () { TableName = "EEP_PERM", Code = "69", Name = "Consulta IVA compras", Descripcion = "Consulta de IVA compras" },
        new () { TableName = "EEP_PERM", Code = "70", Name = "Consulta IVA ventas", Descripcion = "Consulta de IVA ventas" },
        new () { TableName = "EEP_PERM", Code = "71", Name = "Libro de IVA ventas", Descripcion = "Consulta del libro de IVA ventas" },
        new () { TableName = "EEP_PERM", Code = "72", Name = "Libro de IVA compras", Descripcion = "Consulta del libro de IVA compras" },
        new () { TableName = "EEP_PERM", Code = "73", Name = "Reg. de comprobantes Resol. 90/21", Descripcion = "Registro de comprobantes según Resolución 90/21" },
        new () { TableName = "EEP_PERM", Code = "74", Name = "Costo de loteamientos", Descripcion = "Consulta de costos de loteamientos" },
        new () { TableName = "EEP_PERM", Code = "75", Name = "Comparativo de costo e/factura y asiento", Descripcion = "Comparativo de costos entre factura y asiento contable" },
        new () { TableName = "EEP_PERM", Code = "76", Name = "Actualizador masivo de lotes", Descripcion = "Acceso a la actualización masiva de lotes" },
        new () { TableName = "EEP_PERM", Code = "77", Name = "Flujo de cobranzas", Descripcion = "Consulta del flujo de cobranzas" },
        new () { TableName = "EEP_PERM", Code = "78", Name = "Mantenimiento de conceptos", Descripcion = "Acceso al mantenimiento de conceptos" },
        new () { TableName = "EEP_PERM", Code = "79", Name = "Libro mayor", Descripcion = "Consulta del libro mayor" },

        // Roles
        new () { TableName = "EEP_ROLC", Code = "01", Name = "Administrador", Activo = YesNo.Yes },
        new () { TableName = "EEP_ROLC", Code = "02", Name = "Usuario", Activo = YesNo.Yes },
    }.ForEach(AgregarDatosMaestros);
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

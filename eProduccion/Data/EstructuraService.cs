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

using RestSharp;
using System.Data.Odbc;
using eProduccion.Models;

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
            await Task.Delay(100);

            Console.WriteLine($"BD: {_connectionService.DataBase}");
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
                    case "EP_PERM":
                        body = new
                        {
                            Code = masterData.Code,
                            Name = masterData.Name,
                            U_DESC = masterData.Descripcion
                        };
                        break;

                    case "EP_ROLC":
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

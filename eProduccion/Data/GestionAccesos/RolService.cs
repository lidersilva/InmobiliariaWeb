using eProduccion.Models;
using eProduccion.Utility;
using RestSharp;
using System.Data.Odbc;

namespace eProduccion.Data.GestionAccesos
{
    public class RolService(ConnectionService connectionService, DbHelper dbHelper)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly DbHelper _dbHelper = dbHelper;

        public Task<Rol[]> ObtenerRoles()
        {
            var list = new List<Rol>();

            var query = $"SELECT " +
                $"\"Code\", \n" +
                $"\"Name\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_ROLC\" \n" +
                $"ORDER BY \"Name\"; ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Rol();
                che.Codigo = reader["Code"].ToString();
                che.Descripcion = reader["Name"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public Task<Permiso[]> ObtenerPermisosSegunRol(string codigoRol)
        {
            var list = new List<Permiso>();

            var query = $"SELECT " +
                    $"(SELECT R0.\"Code\" FROM \"{_connectionService.DataBase}\".\"@EEP_ROLD\" R0 WHERE R0.\"U_PERM\" = T0.\"Code\" AND R0.\"Code\" = '{codigoRol}') AS \"Code\", " +
                    $"T0.\"Code\" AS \"U_PERM\", T0.\"Name\" AS \"U_PERMDESC\", " +
                    $"(SELECT '1' FROM \"{_connectionService.DataBase}\".\"@EEP_ROLD\" R0 WHERE R0.\"U_PERM\" = T0.\"Code\" AND R0.\"Code\" = '{codigoRol}') AS \"Checked\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"@EEP_PERM\" T0 " +
                    "ORDER BY T0.\"Name\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Permiso();
                che.CodigoRolPermiso = reader["Code"].ToString();
                che.Codigo = reader["U_PERM"].ToString();
                che.Descripcion = reader["U_PERMDESC"].ToString();
                che.Checked = !String.IsNullOrEmpty(reader["Checked"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public async Task GuardarPermisosSegunRol(Rol rol)
        {
            var method = Method.Patch;
            var entity = $"EEP_ROLC('{rol.Codigo}')";

            var body = new
            {
                Code = rol.Codigo,
                Name = rol.Descripcion,
                U_ACTI = rol.Activo,
                EEP_ROLDCollection = rol.RolDetalle
            };

            _connectionService.SetEntitySL(method, entity, body, true);
        }

        public async Task GuardarEditarRol(string accion, string codigo, string rol)
        {
            codigo = accion == "Crear" ? _dbHelper.ObtenerSiguienteCodigo("@EEP_ROLC") : codigo;

            var method = accion == "Crear" ? Method.Post : Method.Patch;
            var entity = accion == "Crear" ? $"EEP_ROLC" : $"EEP_ROLC('{codigo}')";

            // Se crea diccionario para seleccionar qué campos incorporar al JSON
            var body = new Dictionary<string, object>();

            if (accion == "Crear")
            {
                body["Code"] = codigo;
                body["U_ACTI"] = "Y";
            }
            body["Name"] = rol;

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task EliminarRol(string codigo)
        {
            var method = Method.Delete;
            var entity = $"EEP_ROLC('{codigo}')";

            var body = new { };

            _connectionService.SetEntitySL(method, entity, body);
        }
    }
}
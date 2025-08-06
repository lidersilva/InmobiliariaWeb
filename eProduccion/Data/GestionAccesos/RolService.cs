using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;

namespace eProduccion.Data.GestionAccesos
{
    public class RolService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

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
    }
}
using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;

namespace eProduccion.Data.GestionAccesos
{
    public class UsuarioRolService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public Task<Usuario[]> ObtenerUsuarios()
        {
            var list = new List<Usuario>();

            var query = $"SELECT " +
                $"\"U_EXUCODE\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EMUSUA\" \n" +
                $"WHERE \"U_EXUCODE\"!='manager' \n" +
                $"ORDER BY \"U_EXUCODE\"; ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Usuario();
                che.CodigoUsuario = reader["U_EXUCODE"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public Task<UsuarioRol[]> ObtenerRolesSegunUsuario(string codigoUsuario = "")
        {
            var list = new List<UsuarioRol>();

            var query = "SELECT " +
                    $"(SELECT R0.\"Code\" FROM \"{_connectionService.DataBase}\".\"@EEP_ROLU\" R0 WHERE R0.\"U_ROLID\" = T0.\"Code\" AND R0.\"U_USER\" = '{codigoUsuario}') AS \"Code\"," +
                    $"T0.\"Code\" AS \"U_ROLID\", T0.\"Name\" AS \"U_ROL\", " +
                    $"(SELECT '1' FROM \"{_connectionService.DataBase}\".\"@EEP_ROLU\" R0 WHERE R0.\"U_ROLID\" = T0.\"Code\" AND R0.\"U_USER\" = '{codigoUsuario}') AS \"Checked\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"@EEP_ROLC\" T0 " +
                    $"ORDER BY T0.\"Name\"";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new UsuarioRol();
                che.CodigoRolUsuario = reader["Code"].ToString();
                che.CodigoRol = reader["U_ROLID"].ToString();
                che.Rol = reader["U_ROL"].ToString();
                che.Checked = !String.IsNullOrEmpty(reader["Checked"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public async Task GuardarRolUsuario(UsuarioRol usuarioRol, string usuario)
        {
            var code = ObtenerSiguienteCodigo("@EEP_ROLU");

            var method = Method.Post;
            var entity = $"EEP_ROLU";

            var body = new
            {
                Code = code,
                U_USER = usuario,
                U_ROLID = usuarioRol.CodigoRol
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task EliminarRolUsuario(UsuarioRol usuarioRol, string usuario)
        {
            var code = ObtenerCodeUsuarioRol(usuario, usuarioRol.CodigoRol);

            var method = Method.Delete;
            var entity = $"EEP_ROLU('{code}')";

            _connectionService.SetEntitySL(method, entity);
        }

        public string ObtenerCodeUsuarioRol(string usuario, string idRol)
        {
            var codeUsuarioRol = "0";

            var query = $"SELECT \"Code\" FROM \"{_connectionService.DataBase}\".\"@EEP_ROLU\" WHERE \"U_USER\" = '{usuario}' and \"U_ROLID\" = '{idRol}' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                codeUsuarioRol = reader["Code"].ToString();
            }

            _connectionService.DisconnectODBC();

            return codeUsuarioRol;
        }

        public string ObtenerSiguienteCodigo(string tabla)
        {
            var valorSiguiente = "0";

            var query = $"SELECT IFNULL(MAX(CAST(\"Code\" AS INTEGER)), 0) AS \"Code\" " +
                    $"FROM \"{_connectionService.DataBase}\".\"{tabla}\"";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            var valorActual = "0";

            while (reader.Read())
            {
                valorActual = reader["Code"].ToString();
            }

            valorSiguiente = (int.Parse(valorActual) + 1).ToString().PadLeft(2, '0');

            _connectionService.DisconnectODBC();

            return valorSiguiente;
        }
    }
}

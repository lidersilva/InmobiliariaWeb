using RestSharp;
using System.Data.Odbc;
using eProduccion.Models;

namespace eProduccion.Data.GestionAccesos
{
    public class PermisoService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public Task<Permiso[]> ObtenerPermisos()
        {
            var list = new List<Permiso>();

            var query = $"SELECT " +
                $"\"Name\", \n" +
                $"\"U_DESC\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PERM\" \n" +
                $"ORDER BY \"Name\"; ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Permiso();
                che.Nombre = reader["Name"].ToString();
                che.Descripcion = reader["U_DESC"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }
    }
}

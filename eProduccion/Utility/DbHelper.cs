using eProduccion.Data;
using System.Data.Odbc;

namespace eProduccion.Utility
{
    public class DbHelper
    {
        private readonly ConnectionService _connectionService;

        public DbHelper(ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public string ObtenerSiguienteCodigo(string tabla)
        {
            var valorSiguiente = "0";

            var query = $"SELECT IFNULL(MAX(CAST(\"Code\" AS INTEGER)), 0) AS \"Code\" " +
                        $"FROM \"{_connectionService.DataBase}\".\"{tabla}\"";

            using var connection = _connectionService.ConnectODBC();
            using var command = new OdbcCommand(query, connection);
            using var reader = command.ExecuteReader();

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

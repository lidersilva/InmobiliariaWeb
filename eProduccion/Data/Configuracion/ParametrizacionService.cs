using eProduccion.Models;
using System.Data.Odbc;

namespace eProduccion.Data.Configuracion
{
    public class ParametrizacionService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public async Task<Parametrizacion> ObtenerParametrizacion()
        {
            var parametrizacion = new Parametrizacion();

            var query = $"SELECT * " +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PARAM\" TP \n" +
                $"JOIN \"{_connectionService.DataBase}\".\"@EEP_PARSERIE_DET\" TDS ON TP.\"DocEntry\"=TDS.\"DocEntry\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Parametrizacion();

            }

            _connectionService.DisconnectODBC();

            return await Task.FromResult(parametrizacion);
        }

        public Task<Serie[]> ObtenerSeriesOV()
        {
            var list = new List<Serie>();

            var query = $"SELECT \"Series\", \"SeriesName\" FROM \"{_connectionService.DataBase}\".NNM1 WHERE \"ObjectCode\"='17' ORDER BY \"SeriesName\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Serie();
                che.CodigoSerie = int.Parse(reader["Series"].ToString());
                che.Descripcion = reader["SeriesName"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }
    }
}

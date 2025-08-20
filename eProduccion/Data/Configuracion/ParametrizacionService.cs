using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;

namespace eProduccion.Data.Configuracion
{
    public class ParametrizacionService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public async Task<Parametrizacion> ObtenerParametrizacion()
        {
            var parametrizacion = new Parametrizacion();
            var listSeriesDet = new List<SerieDetalle>();
            bool banderaCabecera = true;

            var query = $"SELECT \n" +
                $"TP.\"DocEntry\", \n" +
                $"TDS.\"LineId\", \n" +
                $"TDS.\"U_CODSERIE\", \n" +
                $"TDS.\"U_SERIE\" " +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PARAM\" TP \n" +
                $"LEFT JOIN \"{_connectionService.DataBase}\".\"@EEP_PARSERIE_DET\" TDS ON TP.\"DocEntry\"=TDS.\"DocEntry\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (banderaCabecera)
                    parametrizacion.DocEntry = int.Parse(reader["DocEntry"].ToString());

                var serieDet = new SerieDetalle();
                serieDet.LineId = int.Parse(reader["LineId"].ToString());
                serieDet.CodigoSerie = int.Parse(reader["U_CODSERIE"].ToString());
                serieDet.Serie = reader["U_SERIE"].ToString();
                listSeriesDet.Add(serieDet);
            }

            parametrizacion.SerieDetalle = listSeriesDet;

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

        public List<SerieDetalle> ObtenerSeriesParametrizacion()
        {
            var list = new List<SerieDetalle>();

            var query = $"SELECT \"U_CODSERIE\" FROM \"{_connectionService.DataBase}\".\"@EEP_PARSERIE_DET\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new SerieDetalle();
                che.CodigoSerie = int.Parse(reader["U_CODSERIE"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public async Task GuardarParametrizacion(Parametrizacion parametrizacion)
        {
            var method = parametrizacion.DocEntry == null
                    ? Method.Post : Method.Patch;

            var entity = method == Method.Post
                ? $"EEP_PARAM" : $"EEP_PARAM({parametrizacion.DocEntry})";

            var listSeriesDet = new List<dynamic>();
            foreach (var item in parametrizacion.SerieDetalle)
            {
                var bodyDet = new
                {
                    U_CODSERIE = item.CodigoSerie,
                    U_SERIE = item.Serie,
                };

                listSeriesDet.Add(bodyDet);
            }

            var body = new
            {
                EEP_PARSERIE_DETCollection = listSeriesDet
            };

            _connectionService.SetEntitySL(method, entity, body, true);
        }
    }
}

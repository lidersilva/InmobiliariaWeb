using eProduccion.Models;
using System.Data.Odbc;

namespace eProduccion.Data.Produccion
{
    public class InyeccionService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public Task<OTInyeccion[]> ObtenerOTInyeccion()
        {
            var list = new List<OTInyeccion>();

            var query = $"SELECT \n" +
                $"TI.\"DocEntry\", \n" +
                $"TI.\"U_FECHAOT\", \n" +
                $"TI.\"U_CODARTICULO\", \n" +
                $"(SELECT x.\"ItemName\" FROM \"{_connectionService.DataBase}\".OITM x WHERE x.\"ItemCode\"=TI.\"U_CODARTICULO\") ARTICULO, \n" +
                $"TI.\"U_CODSUBART\", \n" +
                $"TA.\"ItemName\", \n" +
                $"TI.\"U_CANTIDADOT\", \n" +
                $"TP.\"U_DOCNUMOV\", \n" +
                $"TS.\"SeriesName\", \n" +
                $"TI.\"U_ESTADO\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_OT_INY_CAB\" TI \n" +
                $"JOIN \"{_connectionService.DataBase}\".\"@EEP_PLANI_OT\" TP ON TI.\"U_CODEPLANIOT\"=TP.\"Code\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".NNM1 TS ON TP.\"U_CODSERIE\"=TS.\"Series\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".OITM TA ON TI.\"U_CODSUBART\"=TA.\"ItemCode\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTInyeccion();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.FechaOT = DateTime.Parse(reader["U_FECHAOT"].ToString());
                che.CodArticuloOV = reader["U_CODARTICULO"].ToString();
                che.ArticuloOV = reader["ARTICULO"].ToString();
                che.CodArticuloI = reader["U_CODSUBART"].ToString();
                che.ArticuloI = reader["ItemName"].ToString();
                che.CantidadOT = double.Parse(reader["U_CANTIDADOT"].ToString());
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.SerieOV = reader["SeriesName"].ToString();
                che.EstadoOT = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }
    }
}

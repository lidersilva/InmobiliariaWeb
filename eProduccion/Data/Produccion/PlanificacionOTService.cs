using eProduccion.Models;
using System.Data;
using System.Data.Odbc;

namespace eProduccion.Data.Produccion
{
    public class PlanificacionOTService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public async Task EjecutarSP_INSERT_PLANIFICACION_OT()
        {
            var query = $"CALL \"{_connectionService.DataBase}\".\"SP_INSERT_PLANIFICACION_OT\"()";

            var command = new OdbcCommand("", _connectionService.ConnectODBC());
            command.CommandText = query;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();

            _connectionService.DisconnectODBC();
        }

        public Task<PlanificacionOT[]> ObtenerOV()
        {
            var list = new List<PlanificacionOT>();

            var query = $"SELECT \n" +
                $"TP.\"U_FECHAOV\", \n" +
                $"TS.\"SeriesName\", \n" +
                $"TP.\"U_DOCNUMOV\", \n" +
                $"TP.\"U_CODARTICULO\", \n" +
                $"TA.\"ItemName\", \n" +
                $"TP.\"U_CANTIDADOV\", \n" +
                $"TP.\"U_ESTADO\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PLANI_OT\" TP \n" +
                $"JOIN \"{_connectionService.DataBase}\".NNM1 TS ON TP.\"U_SERIE\"=TS.\"Series\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".OITM TA ON TP.\"U_CODARTICULO\"=TA.\"ItemCode\" \n";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new PlanificacionOT();
                che.FechaOV = DateTime.Parse(reader["U_FECHAOV"].ToString());
                che.Serie = reader["SeriesName"].ToString();
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.CodArticulo = reader["U_CODARTICULO"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.CantidadOV = double.Parse(reader["U_CANTIDADOV"].ToString());
                che.EstadoOT = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }
    }
}

using eProduccion.Models;
using System.Data.Odbc;

namespace eProduccion.Data.Produccion
{
    public class EnsambleService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public Task<List<OTEnsamble>> ObtenerOTEnsamblado(string estacion)
        {
            var list = new List<OTEnsamble>();

            var query = $@"
                SELECT 
                TI.""DocEntry"", 
                TI.""U_FECHAOT"", 
                TI.""U_CODSUBART"", 
                TA.""ItemName"", 
                TP.""U_DOCNUMOV"", 
                TS.""SeriesName"", 
                TI.""U_CODEPLANIOT""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_ENSAM_CAB"" TI 
                JOIN ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" TP ON TI.""U_CODEPLANIOT""=TP.""Code"" 
                JOIN ""{_connectionService.DataBase}"".NNM1 TS ON TP.""U_CODSERIE""=TS.""Series"" 
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TI.""U_CODSUBART""=TA.""ItemCode"" 
                WHERE ""U_ESTACION""='{estacion}'
                ORDER BY TI.""DocEntry"" DESC ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTEnsamble();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.FechaOT = DateTime.Parse(reader["U_FECHAOT"].ToString());
                che.CodArticuloEnsamble = reader["U_CODSUBART"].ToString();
                che.ArticuloEnsamble = reader["ItemName"].ToString();
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.SerieOV = reader["SeriesName"].ToString();
                che.CodePlanificacionOT = int.Parse(reader["U_CODEPLANIOT"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToList());
        }
    }
}

using eProduccion.Data.Configuracion;
using eProduccion.Models;
using RestSharp;
using System.Data;
using System.Data.Odbc;
using System.Globalization;

namespace eProduccion.Data.Produccion
{
    public class PlanificacionEnsambleService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public Task<PlanificacionEnsamble[]> ObtenerOV()
        {
            var list = new List<PlanificacionEnsamble>();

            var query = $@"
                SELECT  
                TP.""U_FECHAOV"", 
                TS.""SeriesName"", 
                TP.""U_DOCNUMOV"", 
                TP.""U_CODARTICULO"", 
                TA.""ItemName"", 
                TP.""U_CANTIDADOV""
                FROM ""{_connectionService.DataBase}"".""@EEP_ENSAM_CAB"" TE
                JOIN ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" TP ON TE.""U_CODEPLANIOT""=TP.""Code""
                JOIN ""{_connectionService.DataBase}"".NNM1 TS ON TP.""U_CODSERIE""=TS.""Series"" 
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TP.""U_CODARTICULO""=TA.""ItemCode"" 
                JOIN ""{_connectionService.DataBase}"".ORDR TO ON TP.""U_DOCENTRYOV""=TO.""DocEntry"" 
                JOIN ""{_connectionService.DataBase}"".OITT TM ON TP.""U_CODARTICULO""=TM.""Code"" 
                WHERE TO.""CANCELED""='N' 
                ORDER BY TP.""U_DOCENTRYOV"" DESC";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new PlanificacionEnsamble();
                che.FechaOV = DateTime.Parse(reader["U_FECHAOV"].ToString());
                che.Serie = reader["SeriesName"].ToString();
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.CodArticulo = reader["U_CODARTICULO"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.CantidadOV = double.Parse(reader["U_CANTIDADOV"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }


    }
}

using eProduccion.Data.Configuracion;
using eProduccion.Models;
using eProduccion.Utility;
using RestSharp;
using System.Data.Odbc;
using System.Text;

namespace eProduccion.Data.Produccion
{
    public class PlanificacionEnsambleService(ConnectionService connectionService, ParametrizacionService parametrizacionService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly ParametrizacionService _parametrizacionService = parametrizacionService;

        public Task<PlanificacionEnsamble[]> ObtenerPlanificacionEnsamble()
        {
            var list = new List<PlanificacionEnsamble>();

            var query = $@"
                SELECT  
                TE.""DocEntry"", 
                TP.""U_FECHAOV"", 
                TS.""SeriesName"", 
                TP.""U_DOCNUMOV"", 
                TP.""U_CODARTICULO"", 
                TA.""ItemName"", 
                TP.""U_CANTIDADOV"",
                TE.""U_ESTANTERIOR"",
                TE.""U_CODEPLANIOT""
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
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.FechaOV = DateTime.Parse(reader["U_FECHAOV"].ToString());
                che.Serie = reader["SeriesName"].ToString();
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.CodArticulo = reader["U_CODARTICULO"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.CantidadOV = double.Parse(reader["U_CANTIDADOV"].ToString());
                che.EstacionAnterior = reader["U_ESTANTERIOR"].ToString();
                che.CodePlaniOT = int.Parse(reader["U_CODEPLANIOT"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public Task<List<PlanificacionEnsambleDet>> ObtenerDetalleEnsamble(int docEntryOT)
        {
            var list = new List<PlanificacionEnsambleDet>();

            var query = $@"
                SELECT
                TD.""LineId"",
                TS.""SeriesName"", 
                TP.""U_DOCNUMOV"",
                TP.""U_FECHAOV"",
                TD.""U_CANTPROD"",
                TD.""U_CANTPRODKG"",
                TD.""U_OT"",
                TD.""U_CODSUBART"",
                TA.""ItemName"", 
                (SELECT x.""OnHand"" FROM ""{_connectionService.DataBase}"".OITW x WHERE x.""ItemCode""=TD.""U_CODSUBART"" AND x.""WhsCode""='01') ""Stock"",
                TD.""U_ESTADO"",
                TD.""U_CANTSOLICITADA""
                FROM ""{_connectionService.DataBase}"".""@EEP_ENSAM_CAB"" TC
                JOIN ""{_connectionService.DataBase}"".""@EEP_ENSAM_DET"" TD ON TC.""DocEntry""=TD.""DocEntry""
                JOIN ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" TP ON TC.""U_CODEPLANIOT""=TP.""Code""
                JOIN ""{_connectionService.DataBase}"".NNM1 TS ON TP.""U_CODSERIE""=TS.""Series""
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TP.""U_CODARTICULO""=TA.""ItemCode""
                WHERE TC.""DocEntry"" = {docEntryOT}
                ORDER BY TD.""U_OT"", TD.""LineId"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new PlanificacionEnsambleDet();
                che.LineId = int.Parse(reader["LineId"].ToString());
                che.SerieOV = reader["SeriesName"].ToString();
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.FechaOV = DateTime.Parse(reader["U_FECHAOV"].ToString());
                che.CantProducida = int.Parse(reader["U_CANTPROD"].ToString());
                che.CantProducidaKG = double.Parse(reader["U_CANTPRODKG"].ToString());
                che.OT = int.Parse(reader["U_OT"].ToString());
                che.CodArticuloI = reader["U_CODSUBART"].ToString();
                che.ArticuloI = reader["ItemName"].ToString();
                che.Stock = double.Parse(reader["Stock"].ToString());
                che.Estado = reader["U_ESTADO"].ToString();
                che.CantSolicitada = int.Parse(reader["U_CANTSOLICITADA"].ToString());
                che.CantDisponible = che.CantProducida - che.CantSolicitada;
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToList());
        }

        public Task<List<Lote>> ObtenerLotesEnsamble(string codArticuloI, string almacen)
        {
            var list = new List<Lote>();

            var query = $@"
                SELECT 
                TC.""DistNumber"", 
                TD.""Quantity""
                FROM ""{_connectionService.DataBase}"".OBTQ TD 
                JOIN ""{_connectionService.DataBase}"".OBTN TC ON TD.""SysNumber""=TC.""SysNumber"" AND TD.""ItemCode"" = TC.""ItemCode"" 
                WHERE TD.""Quantity"">0 
                AND TD.""ItemCode""='{codArticuloI}' 
                AND TD.""WhsCode"" ='{almacen}' 
                GROUP BY TC.""DistNumber"", TD.""Quantity"", TC.""InDate""
                ORDER BY IFNULL(TC.""InDate"", CURRENT_DATE) ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Lote();
                che.NroLote = reader["DistNumber"].ToString();
                che.Cantidad = double.Parse(reader["Quantity"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToList());
        }

        public async Task GenerarOT(List<PlanificacionEnsambleDet> listPlanifEnsambleDet, string codigoArticuloOV, int codePlanificacionOT, int docEntryOT)
        {
            StringBuilder bodyBatch = new StringBuilder();

            var method = Method.Post;
            var entity = $"$batch";

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--Batch_Boundary_EEP");
            bodyBatch.AppendLine("content-type: multipart/mixed;boundary=changeset_EEP");
            bodyBatch.AppendLine();

            // Generar OT según la lista de materiales
            bodyBatch.AppendLine(await GenerarOTSegunListaMateriales(listPlanifEnsambleDet, codigoArticuloOV, codePlanificacionOT));

            // Actualizar líneas iniciadas del detalle de planificación de ensamble
            bodyBatch.AppendLine(ActualizarLineaPlanificacionEnsamble(listPlanifEnsambleDet, docEntryOT));

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--changeset_EEP--");
            bodyBatch.AppendLine("--Batch_Boundary_EEP--");

            _connectionService.SetEntitySLBatch(method, entity, bodyBatch);
        }

        private async Task<string> GenerarOTSegunListaMateriales(List<PlanificacionEnsambleDet> listPlanifEnsambleDet, string codigoArticuloOV, int codePlanificacionOT)
        {
            StringBuilder bodyBatch = new StringBuilder();
            var parametrizacion = await _parametrizacionService.ObtenerParametrizacion();

            var sigEtapaRuta = ObtenerEtapasRuta(codigoArticuloOV);

            if (sigEtapaRuta.EtapaRuta == parametrizacion.CodEstacionArmado)
            {
                bodyBatch.AppendLine("--changeset_EEP");
                bodyBatch.AppendLine("content-type: application/http");
                bodyBatch.AppendLine("content-transfer-encoding:binary");
                bodyBatch.AppendLine();
                bodyBatch.AppendLine($"POST /b1s/v1/EEP_OT_ENSAM_CAB");
                bodyBatch.AppendLine();

                var listRegistroEnsamblado = new List<dynamic>();
                foreach (var i in listPlanifEnsambleDet)
                {
                    var bodyDet = new
                    {
                        U_CODSUBART = i.CodArticuloI,
                        U_LOTE = i.Lote,
                        U_CANTIDAD = i.CantSolicitar,
                    };

                    listRegistroEnsamblado.Add(bodyDet);
                }

                var body = new
                {
                    U_CODEPLANIOT = codePlanificacionOT,
                    U_ESTACION = "ARMADO",
                    U_FECHAOT = DateTime.Now.ToString("yyyy-MM-dd"),
                    U_USEROT = _connectionService.UserName,
                    U_CODSUBART = sigEtapaRuta.SubProducto,
                    EEP_REG_ENSAM_DETCollection = listRegistroEnsamblado,
                };

                bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
                bodyBatch.AppendLine();
            }

            return bodyBatch.ToString();
        }

        private ListaMateriales ObtenerEtapasRuta(string codigoArticulo)
        {
            var etapaRuta = new ListaMateriales();

            var query = $@"
                SELECT TOP 1
                    T1.""Code"", 
                    T0.""U_CodAcabado""
                FROM ""{_connectionService.DataBase}"".ITT2 T0 
                JOIN ""{_connectionService.DataBase}"".ORST T1 ON T0.""StgEntry"" = T1.""AbsEntry""
                WHERE 
                    T0.""Father"" = '{codigoArticulo}'
                    AND T0.""U_Tipo"" = 'E'
                ORDER BY T0.""SeqNum"" ASC ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                etapaRuta.EtapaRuta = reader["Code"].ToString();
                etapaRuta.SubProducto = reader["U_CodAcabado"].ToString();
            }

            _connectionService.DisconnectODBC();

            return etapaRuta;
        }

        private string ActualizarLineaPlanificacionEnsamble(List<PlanificacionEnsambleDet> listPlanifEnsambleDet, int docEntryOT)
        {
            StringBuilder bodyBatch = new StringBuilder();

            bodyBatch.AppendLine("--changeset_EEP");
            bodyBatch.AppendLine("content-type: application/http");
            bodyBatch.AppendLine("content-transfer-encoding:binary");
            bodyBatch.AppendLine();
            bodyBatch.AppendLine($"PATCH /b1s/v1/EEP_ENSAM_CAB({docEntryOT})");
            bodyBatch.AppendLine();

            var listRegistroEnsamblado = new List<dynamic>();
            foreach (var i in listPlanifEnsambleDet)
            {
                var bodyDet = new
                {
                    LineId = i.LineId,
                    U_ESTADO = "En proceso",
                    U_CANTSOLICITADA = i.CantSolicitada + i.CantSolicitar,
                };

                listRegistroEnsamblado.Add(bodyDet);
            }

            var body = new
            {
                EEP_ENSAM_DETCollection = listRegistroEnsamblado,
            };

            bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
            bodyBatch.AppendLine();

            return bodyBatch.ToString();
        }
    }
}

using eProduccion.Data.Configuracion;
using eProduccion.Models;
using eProduccion.Utility;
using RestSharp;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace eProduccion.Data.Produccion
{
    public class PlanificacionOTService(ConnectionService connectionService, ParametrizacionService parametrizacionService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly ParametrizacionService _parametrizacionService = parametrizacionService;

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
            var seriesParametrizacion = _parametrizacionService.ObtenerSeriesParametrizacion();
            var listaSeriesParametrizacion = string.Join(", ", seriesParametrizacion.Select(x => x.CodigoSerie));

            var query = $"SELECT \n" +
                $"TP.\"Code\", \n" +
                $"TP.\"U_FECHAOV\", \n" +
                $"TS.\"SeriesName\", \n" +
                $"TP.\"U_DOCENTRYOV\", \n" +
                $"TP.\"U_DOCNUMOV\", \n" +
                $"TP.\"U_CODARTICULO\", \n" +
                $"TA.\"ItemName\", \n" +
                $"TP.\"U_CANTIDADOV\", \n" +
                $"TP.\"U_ESTADO\", \n" +
                $"TP.\"U_CANTSOLICITADA\" \n" +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PLANI_OT\" TP \n" +
                $"JOIN \"{_connectionService.DataBase}\".NNM1 TS ON TP.\"U_CODSERIE\"=TS.\"Series\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".OITM TA ON TP.\"U_CODARTICULO\"=TA.\"ItemCode\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".ORDR TO ON TP.\"U_DOCENTRYOV\"=TO.\"DocEntry\" \n" +
                $"JOIN \"{_connectionService.DataBase}\".OITT TM ON TP.\"U_CODARTICULO\"=TM.\"Code\" \n" +
                $"WHERE TO.\"CANCELED\"='N' \n" +
                $"AND TP.\"U_CODSERIE\" IN ({listaSeriesParametrizacion}) \n" +
                $"ORDER BY TP.\"U_DOCENTRYOV\" DESC \n";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new PlanificacionOT();
                che.Code = int.Parse(reader["Code"].ToString());
                che.FechaOV = DateTime.Parse(reader["U_FECHAOV"].ToString());
                che.Serie = reader["SeriesName"].ToString();
                che.DocEntryOV = int.Parse(reader["U_DOCENTRYOV"].ToString());
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.CodArticulo = reader["U_CODARTICULO"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.CantidadOV = double.Parse(reader["U_CANTIDADOV"].ToString());
                che.EstadoOT = reader["U_ESTADO"].ToString();
                che.CantSolicitada = double.Parse(reader["U_CANTSOLICITADA"].ToString());
                che.CantDisponible = che.CantidadOV - che.CantSolicitada;
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public async Task GenerarOT(List<PlanificacionOT> listaOV)
        {
            StringBuilder bodyBatch = new StringBuilder();

            var method = Method.Post;
            var entity = $"$batch";

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--Batch_Boundary_EEP");
            bodyBatch.AppendLine("content-type: multipart/mixed;boundary=changeset_EEP");
            bodyBatch.AppendLine();

            // Generar OT para las diferentes estaciones según la lista de materiales
            bodyBatch.AppendLine(await GenerarOTSegunListaMateriales(listaOV));

            // Actualizar líneas iniciadas en EEP_PLANI_OT (Tabla planificación OT)
            bodyBatch.AppendLine(ActualizarLineaPlanificacion(listaOV));

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--changeset_EEP--");
            bodyBatch.AppendLine("--Batch_Boundary_EEP--");

            _connectionService.SetEntitySLBatch(method, entity, bodyBatch);
        }

        private string ActualizarLineaPlanificacion(List<PlanificacionOT> listaOV)
        {
            StringBuilder bodyBatch = new StringBuilder();

            foreach (var i in listaOV)
            {
                bodyBatch.AppendLine("--changeset_EEP");
                bodyBatch.AppendLine("content-type: application/http");
                bodyBatch.AppendLine("content-transfer-encoding:binary");
                bodyBatch.AppendLine($"Content-ID: {i.Code}");
                bodyBatch.AppendLine();
                bodyBatch.AppendLine($"PATCH /b1s/v1/U_EEP_PLANI_OT({i.Code})");
                bodyBatch.AppendLine();

                var body = new
                {
                    U_ESTADO = "En proceso",
                    U_CANTSOLICITADA = i.CantSolicitada + i.CantSolicitar,
                    U_FECHAOT = DateTime.Now.ToString("yyyy-MM-dd"),
                    U_HORAOT = DateTime.Now.ToString("HHmm"),
                    U_USEROT = _connectionService.UserName
                };

                bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
                bodyBatch.AppendLine();
            }

            return bodyBatch.ToString();
        }

        private List<ListaMateriales> ObtenerEtapasRuta(string codigoArticulo)
        {
            var list = new List<ListaMateriales>();

            var query = $"SELECT \n" +
                $"T1.\"Code\", \n" +
                $"T0.\"U_CodAcabado\" \n" +
                $"FROM \"{_connectionService.DataBase}\".ITT2 T0 \n" +
                $"JOIN \"{_connectionService.DataBase}\".ORST T1 ON T0.\"StgEntry\"=T1.\"AbsEntry\" \n" +
                $"WHERE T0.\"Father\"='{codigoArticulo}' \n" +
                $"AND T1.\"Code\" = \n" +
                $"(SELECT x1.\"Code\" \n" +
                $"FROM \"{_connectionService.DataBase}\".ITT2 x0 \n" +
                $"JOIN \"{_connectionService.DataBase}\".ORST x1 ON x0.\"StgEntry\"=x1.\"AbsEntry\" \n" +
                $"WHERE x0.\"Father\"=T0.\"Father\" \n" +
                $"AND x0.\"SeqNum\"=1 ) \n" +
                $"ORDER BY T0.\"SeqNum\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new ListaMateriales();
                che.EtapaRuta = reader["Code"].ToString();
                che.SubProducto = reader["U_CodAcabado"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        private async Task<string> GenerarOTSegunListaMateriales(List<PlanificacionOT> listaOV)
        {
            StringBuilder bodyBatch = new StringBuilder();
            var parametrizacion = await _parametrizacionService.ObtenerParametrizacion();

            foreach (var i in listaOV)
            {
                var listaEtapasRuta = ObtenerEtapasRuta(i.CodArticulo);

                foreach (var e in listaEtapasRuta)
                {
                    if (e.EtapaRuta == parametrizacion.CodEstacionInyeccion)
                    {
                        bodyBatch.AppendLine("--changeset_EEP");
                        bodyBatch.AppendLine("content-type: application/http");
                        bodyBatch.AppendLine("content-transfer-encoding:binary");
                        bodyBatch.AppendLine();
                        bodyBatch.AppendLine($"POST /b1s/v1/EEP_OT_INYEX_CAB");
                        bodyBatch.AppendLine();

                        var body = new
                        {
                            U_FECHAOT = DateTime.Now.ToString("yyyy-MM-dd"),
                            U_CODARTICULO = i.CodArticulo,
                            U_CODSUBART = e.SubProducto,
                            U_CANTIDADOT = i.CantSolicitar,
                            U_ESTADO = "Pendiente",
                            U_DOCENTRYOV = i.DocEntryOV,
                            U_ESTANTERIOR = "PLANIFICACION OT",
                            U_CODEPLANIOT = i.Code,
                            U_USEROT = _connectionService.UserName,
                            U_ESTACION = "INYECCION",
                        };

                        bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
                        bodyBatch.AppendLine();
                    }
                    else if (e.EtapaRuta == parametrizacion.CodEstacionExtrusion)
                    {
                        bodyBatch.AppendLine("--changeset_EEP");
                        bodyBatch.AppendLine("content-type: application/http");
                        bodyBatch.AppendLine("content-transfer-encoding:binary");
                        bodyBatch.AppendLine();
                        bodyBatch.AppendLine($"POST /b1s/v1/EEP_OT_INYEX_CAB");
                        bodyBatch.AppendLine();

                        var body = new
                        {
                            U_FECHAOT = DateTime.Now.ToString("yyyy-MM-dd"),
                            U_CODARTICULO = i.CodArticulo,
                            U_CODSUBART = e.SubProducto,
                            U_CANTIDADOT = i.CantSolicitar,
                            U_ESTADO = "Pendiente",
                            U_DOCENTRYOV = i.DocEntryOV,
                            U_ESTANTERIOR = "PLANIFICACION OT",
                            U_CODEPLANIOT = i.Code,
                            U_USEROT = _connectionService.UserName,
                            U_ESTACION = "EXTRUSION",
                        };

                        bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
                        bodyBatch.AppendLine();
                    }
                }
            }

            return bodyBatch.ToString();
        }
    }
}

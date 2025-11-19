using eProduccion.Data.Configuracion;
using eProduccion.Models;
using eProduccion.Utility;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;
using System.Text;

namespace eProduccion.Data.Produccion
{
    public class MolinadoService(ConnectionService connectionService, ParametrizacionService parametrizacionService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly ParametrizacionService _parametrizacionService = parametrizacionService;

        public Task<PendienteMolinar[]> ObtenerOTPendienteMolinar()
        {
            var list = new List<PendienteMolinar>();

            var query = $@"
                SELECT 
                TM.""DocEntry"", 
                TM.""U_ESTANTERIOR"", 
                TD.""U_FECHAPROC"", 
                TD.""U_HORAINI"", 
                TD.""U_HORAFIN"", 
                TD.""U_TURNO"", 
                TD.""U_OPERARIO"", 
                TD.""U_OPERARIO2"",
                TC.""U_CODSUBART"",
                TA.""ItemName"",
                TM.""U_CANTPROD"",
                TM.""U_CANTSOLICITADA"",
                TM.""U_LOTE""
                FROM ""{_connectionService.DataBase}"".""@EEP_PEND_MOLI_OT"" TM 
                JOIN ""{_connectionService.DataBase}"".""@EEP_OT_INYEX_DET"" TD ON TM.""U_OT""=TD.""DocEntry"" AND TM.""U_LINEIDOT"" =""LineId""
                JOIN ""{_connectionService.DataBase}"".""@EEP_OT_INYEX_CAB"" TC ON TD.""DocEntry""=TC.""DocEntry""
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TC.""U_CODSUBART""=TA.""ItemCode""
                ORDER BY TM.""DocEntry"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new PendienteMolinar();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.EstacionOrigen = reader["U_ESTANTERIOR"].ToString();
                che.Fecha = DateTime.Parse(reader["U_FECHAPROC"].ToString());
                var horaInicioString = reader["U_HORAINI"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaInicioString) && horaInicioString.Length == 4)
                    che.HoraInicio = DateTime.ParseExact(horaInicioString, "HHmm", CultureInfo.InvariantCulture);
                var horaFinString = reader["U_HORAFIN"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaFinString) && horaFinString.Length == 4)
                    che.HoraFin = DateTime.ParseExact(horaFinString, "HHmm", CultureInfo.InvariantCulture);
                che.Turno = reader["U_TURNO"].ToString();
                che.Operario = reader["U_OPERARIO"].ToString();
                che.Operario2 = reader["U_OPERARIO2"].ToString();
                che.CodArticulo = reader["U_CODSUBART"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.CantProducida = int.Parse(reader["U_CANTPROD"].ToString());
                che.CantSolicitada = int.Parse(reader["U_CANTSOLICITADA"].ToString());
                che.CantDisponible = che.CantProducida - che.CantSolicitada;
                che.Lote = reader["U_LOTE"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public async Task GenerarOT(List<PendienteMolinar> listaOT)
        {
            StringBuilder bodyBatch = new StringBuilder();

            var method = Method.Post;
            var entity = $"$batch";

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--Batch_Boundary_EEP");
            bodyBatch.AppendLine("content-type: multipart/mixed;boundary=changeset_EEP");
            bodyBatch.AppendLine();

            // Generar OT para las diferentes estaciones según la lista de materiales
            //bodyBatch.AppendLine(await GenerarOTSegunListaMateriales(listaOT));

            bodyBatch.AppendLine(ActualizarOTPendienteMolinar(listaOT));

            bodyBatch.AppendLine();
            bodyBatch.AppendLine("--changeset_EEP--");
            bodyBatch.AppendLine("--Batch_Boundary_EEP--");

            _connectionService.SetEntitySLBatch(method, entity, bodyBatch);
        }

        private string ActualizarOTPendienteMolinar(List<PendienteMolinar> listaOT)
        {
            StringBuilder bodyBatch = new StringBuilder();

            foreach (var i in listaOT)
            {
                bodyBatch.AppendLine("--changeset_EEP");
                bodyBatch.AppendLine("content-type: application/http");
                bodyBatch.AppendLine("content-transfer-encoding:binary");
                bodyBatch.AppendLine($"Content-ID: {i.DocEntry}");
                bodyBatch.AppendLine();
                bodyBatch.AppendLine($"PATCH /b1s/v1/EEP_PEND_MOLI_OT({i.DocEntry})");
                bodyBatch.AppendLine();

                var body = new
                {
                    U_CANTSOLICITADA = i.CantSolicitada + i.CantSolicitar,
                };

                bodyBatch.AppendLine(Utils.JsonSerializeObject(body));
                bodyBatch.AppendLine();
            }

            return bodyBatch.ToString();
        }
    }
}

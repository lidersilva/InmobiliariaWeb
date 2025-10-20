using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;

namespace eProduccion.Data.Produccion
{
    public class EnsambleService(ConnectionService connectionService, InyeccionExtrusionService inyeccionExtrusionService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly InyeccionExtrusionService _inyeccionExtrusionService = inyeccionExtrusionService;

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

        public Task<List<OTEnsambleDet>> ObtenerDetalleOTEnsamblado(int docEntryOT)
        {
            var list = new List<OTEnsambleDet>();

            var query = $@"
                SELECT 
                    ""LineId"",
                    ""U_NROCONTEN"",
                    ""U_NROMAQUI"",
                    ""U_FECHAPROC"",
                    ""U_HORAINI"",
                    ""U_HORAFIN"",
                    ""U_TURNO"",
                    ""U_OPERARIO"",
                    ""U_OPERARIO2"",
                    IFNULL(""U_CANTAPROB"", 0) ""U_CANTAPROB"",
                    IFNULL(""U_CANTPRODKG"", 0) ""U_CANTPRODKG"",
                    IFNULL(""U_CCP1"", 0) ""U_CCP1"",
                    IFNULL(""U_CANTAPROBD"", 0) ""U_CANTAPROBD"",
                    ""U_OBS"",
                    ""U_LIBERADO"",
                    IFNULL(TS.""DocNum"", 0) ""DocNumSalida"",
                    IFNULL(TD.""U_NROASIENTO"", 0) ""U_NROASIENTO"",
                    IFNULL(TE.""DocNum"", 0) ""DocNumEntrada"",
                    ""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_ENSAM_DET"" TD
                LEFT JOIN  ""{_connectionService.DataBase}"".OIGE TS ON TD.""U_DESALIDA""=TS.""DocEntry""
                LEFT JOIN  ""{_connectionService.DataBase}"".OIGN TE ON TD.""U_DEENTRADA""=TE.""DocEntry""
                WHERE TD.""DocEntry"" = {docEntryOT}
                ORDER BY ""LineId""";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTEnsambleDet();
                che.DocEntry = docEntryOT;
                che.LineId = int.Parse(reader["LineId"].ToString());
                che.NroContenedor = reader["U_NROCONTEN"].ToString();
                che.NroMaquina = reader["U_NROMAQUI"].ToString();
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
                che.CantAprobadas = int.Parse(reader["U_CANTAPROB"].ToString());
                che.CantAprobadasKG = double.Parse(reader["U_CANTPRODKG"].ToString());
                che.PesoPiezaG = double.Parse(reader["U_CCP1"].ToString());
                che.CantAprobadasDesvio = int.Parse(reader["U_CANTAPROBD"].ToString());
                che.Observaciones = reader["U_OBS"].ToString();
                che.Liberado = reader["U_LIBERADO"].ToString() == "Y";
                che.DocNumSalida = int.Parse(reader["DocNumSalida"].ToString());
                che.Asiento = int.Parse(reader["U_NROASIENTO"].ToString());
                che.DocNumEntrada = int.Parse(reader["DocNumEntrada"].ToString());
                che.EstadoLinea = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public Task<List<RegistroEnsambleDet>> ObtenerDetalleRegistroEnsamblado(int docEntryOT)
        {
            var list = new List<RegistroEnsambleDet>();

            var query = $@"
                SELECT
                TD.""LineId"",
                TD.""U_CODSUBART"",
                TA.""ItemName"",
                TD.""U_LOTE"",
                TD.""U_CANTIDAD"",
                TD.""U_TIPO"",
                IFNULL(TD.""U_CANTREP"", 0) ""U_CANTREP""
                FROM ""{_connectionService.DataBase}"".""@EEP_REG_ENSAM_DET"" TD
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TD.""U_CODSUBART""=TA.""ItemCode""
                WHERE TD.""DocEntry""={docEntryOT}
                ORDER BY TD.""LineId"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new RegistroEnsambleDet();
                che.LineId = int.Parse(reader["LineId"].ToString());
                che.CodArticulo = reader["U_CODSUBART"].ToString();
                che.Articulo = reader["ItemName"].ToString();
                che.Lote = reader["U_LOTE"].ToString();
                che.Cantidad = int.Parse(reader["U_CANTIDAD"].ToString());
                che.Tipo = reader["U_TIPO"].ToString();
                che.CantidadReproceso = int.Parse(reader["U_CANTREP"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public async Task RegistrarInicioLineaEnsamblado(int docEntryOT, OTEnsambleDet detOTEnsamble)
        {
            var method = Method.Patch;
            var entity = $"EEP_OT_ENSAM_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_ENSAM_DETCollection = new[]
                {
                    new
                    {
                        LineId = detOTEnsamble.LineId,
                        U_HORAINI = DateTime.Now.ToString("HHmm"),
                        U_ESTADO = "Iniciado",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task GuardarLineaEnsamblado(int docEntryOT, OTEnsambleDet detOTEnsamble)
        {
            var method = Method.Patch;
            var entity = $"EEP_OT_ENSAM_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_ENSAM_DETCollection = new[]
                {
                    new
                    {
                        LineId = detOTEnsamble.LineId,
                        U_NROCONTEN = detOTEnsamble.NroContenedor,
                        U_NROMAQUI = detOTEnsamble.NroMaquina,
                        U_FECHAPROC = detOTEnsamble.Fecha,
                        U_TURNO = detOTEnsamble.Turno,
                        U_OPERARIO = detOTEnsamble.Operario,
                        U_OPERARIO2 = detOTEnsamble.Operario2,
                        U_CANTAPROB = detOTEnsamble.CantAprobadas,
                        U_CANTPRODKG = detOTEnsamble.CantAprobadasKG,
                        U_CCP1 = detOTEnsamble.PesoPiezaG,
                        U_CANTAPROBD = detOTEnsamble.CantAprobadasDesvio,
                        U_OBS = detOTEnsamble.Observaciones,
                        U_ESTADO = "Pendiente",
                        U_LIBERADO = detOTEnsamble.Liberado ? "Y" : "N",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public Task<List<ListaMaterialesDet>> ObtenerArticulosListaMateriales(string estacion, int codePlanificacionOT, string codSubArticulo)
        {
            var codArticuloOV = ObtenerCodArticuloOV(codePlanificacionOT);

            var listaArticulos = _inyeccionExtrusionService.ObtenerListaMateriales(codArticuloOV, codSubArticulo, "09").Where(x => x.TipoItem == 4);

            return Task.FromResult(listaArticulos.ToList());
        }

        private string ObtenerCodArticuloOV(int codePlanificacionOT)
        {
            var codArticuloOV = "";

            var query = $@"SELECT ""U_CODARTICULO"" FROM ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" WHERE ""Code""={codePlanificacionOT} ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                codArticuloOV = reader["U_CODARTICULO"].ToString();
            }

            _connectionService.DisconnectODBC();

            return codArticuloOV;
        }

        public async Task GuardarRegistroEnsamblado(int docEntryOT, List<RegistroEnsambleDet> registroEnsamble)
        {
            var method = Method.Patch;
            var entity = $"EEP_OT_ENSAM_CAB({docEntryOT})";

            var listRegistroEnsamblado = new List<dynamic>();
            foreach (var i in registroEnsamble)
            {
                var bodyDet = new
                {
                    LineId = i.LineId,
                    U_CODSUBART = i.CodArticulo,
                    U_LOTE = i.Lote,
                    U_CANTIDAD = i.Cantidad,
                    U_TIPO = i.Tipo,
                    U_CANTREP = i.CantidadReproceso,
                };

                listRegistroEnsamblado.Add(bodyDet);
            }

            var body = new
            {
                EEP_REG_ENSAM_DETCollection = listRegistroEnsamblado
            };

            _connectionService.SetEntitySL(method, entity, body);
        }
    }
}

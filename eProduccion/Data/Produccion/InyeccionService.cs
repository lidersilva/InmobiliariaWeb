using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;

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
                $"JOIN \"{_connectionService.DataBase}\".OITM TA ON TI.\"U_CODSUBART\"=TA.\"ItemCode\" \n" +
                $"ORDER BY TI.\"DocEntry\" DESC ";

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

        public Task<List<OTInyeccionDet>> ObtenerDetalleInyeccion(int docEntryOT)
        {
            var list = new List<OTInyeccionDet>();

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
                    IFNULL(""U_CANTAPROB"", 0) ""U_CANTAPROB"",
                    IFNULL(""U_CANTRET"", 0) ""U_CANTRET"",
                    IFNULL(""U_CANTMERMA"", 0) ""U_CANTMERMA"",
                    IFNULL(""U_CANTMERMAKG"", 0) ""U_CANTMERMAKG"",
                    ""U_MOTIVOMERMA"",
                    IFNULL(""U_CANTMERMA2"", 0) ""U_CANTMERMA2"",
                    IFNULL(""U_CANTMERMAKG2"", 0) ""U_CANTMERMAKG2"",
                    ""U_MOTIVOMERMA2"",
                    IFNULL(""U_CCP1"", 0) ""U_CCP1"",
                    IFNULL(""U_CCP2"", 0) ""U_CCP2"",
                    IFNULL(""U_CCP3"", 0) ""U_CCP3"",
                    IFNULL(""U_CCP4"", 0) ""U_CCP4"",
                    IFNULL(""U_CCP5"", 0) ""U_CCP5"",
                    IFNULL(""U_CCP6"", 0) ""U_CCP6"",
                    IFNULL(""U_CCP7"", 0) ""U_CCP7"",
                    IFNULL(""U_CCP8"", 0) ""U_CCP8"",
                    ""U_LIBERADO"",
                    IFNULL(""U_DEENTRADA"", 0) ""U_DEENTRADA"",
                    IFNULL(""U_DESALIDA"", 0) ""U_DESALIDA"",
                    ""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_INY_DET"" TD
                WHERE TD.""DocEntry"" = {docEntryOT}
                ORDER BY ""LineId""";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTInyeccionDet();
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
                che.CantAprobadas = int.Parse(reader["U_CANTAPROB"].ToString());
                che.CantRetenidas = int.Parse(reader["U_CANTRET"].ToString());
                che.CantRechReciclable = int.Parse(reader["U_CANTMERMA"].ToString());
                che.PesoRechReciclable = double.Parse(reader["U_CANTMERMAKG"].ToString());
                che.MotiMPesoRechReciclable = reader["U_MOTIVOMERMA"].ToString();
                che.CantRechNoReciclable = int.Parse(reader["U_CANTMERMA2"].ToString());
                che.PesoRechNoReciclable = double.Parse(reader["U_CANTMERMAKG2"].ToString());
                che.MotiMPesoRechNoReciclable = reader["U_MOTIVOMERMA2"].ToString();
                che.PesoColadaKG = double.Parse(reader["U_CCP1"].ToString());
                che.PesoMasacoteKG = double.Parse(reader["U_CCP2"].ToString());
                che.PesoAjusMaquinaKG = double.Parse(reader["U_CCP3"].ToString());
                che.PesoPiezaG = double.Parse(reader["U_CCP4"].ToString());
                che.CavidadReal = int.Parse(reader["U_CCP5"].ToString());
                che.CavidadOperativa = int.Parse(reader["U_CCP6"].ToString());
                che.TiempoCicloReal = double.Parse(reader["U_CCP7"].ToString());
                che.TiempoCiclo = double.Parse(reader["U_CCP8"].ToString());
                che.Liberado = reader["U_LIBERADO"].ToString() == "Y";
                che.DocEntryEntrada = int.Parse(reader["U_DEENTRADA"].ToString());
                che.DocEntrySalida = int.Parse(reader["U_DESALIDA"].ToString());
                che.EstadoLinea = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public Task<List<Parada>> ObtenerRegistroParadas(int docEntryOT, int lineIdOT, string estacionTrabajo)
        {
            var list = new List<Parada>();

            var query = $@"
                SELECT 
                    TP.""DocEntry"", 
                    TP.""U_FECHA"", 
                    TP.""U_TIPOPARO"", 
                    TP.""U_TURNO"", 
                    TP.""U_HORAINI"", 
                    TP.""U_HORAFIN"", 
                    TP.""U_NROMAQUI"",
                    TP.""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_PARADAS"" TP
                WHERE TP.""U_ESTACION""='{estacionTrabajo}'
                AND TP.""U_OT""={docEntryOT}
                AND TP.""U_LINEIDOT""={lineIdOT}
                ORDER BY TP.""DocEntry"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Parada();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.Fecha = DateTime.Parse(reader["U_FECHA"].ToString());
                che.TipoParada = reader["U_TIPOPARO"].ToString();
                che.Turno = reader["U_TURNO"].ToString();
                var horaInicioString = reader["U_HORAINI"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaInicioString) && horaInicioString.Length == 4)
                    che.HoraInicio = DateTime.ParseExact(horaInicioString, "HHmm", CultureInfo.InvariantCulture);
                var horaFinString = reader["U_HORAFIN"].ToString().PadLeft(4, '0');
                if (!string.IsNullOrWhiteSpace(horaFinString) && horaFinString.Length == 4)
                    che.HoraFin = DateTime.ParseExact(horaFinString, "HHmm", CultureInfo.InvariantCulture);
                che.NroMaquina = reader["U_NROMAQUI"].ToString();
                che.Estado = reader["U_ESTADO"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public async Task RegistrarInicioInyeccion(int docEntryOT, OTInyeccionDet detalleInyeccion)
        {
            var fechaActual = DateTime.Now;

            var method = Method.Patch;
            var entity = $"EEP_OT_INY_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_INY_DETCollection = new[]
                {
                    new
                    {
                        LineId = detalleInyeccion.LineId,
                        U_HORAINI = fechaActual.ToString("HHmm"),
                        U_ESTADO = "Iniciado",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task GuardarLineaInyeccion(int docEntryOT, OTInyeccionDet detalleInyeccion)
        {
            var fechaActual = DateTime.Now;

            var method = Method.Patch;
            var entity = $"EEP_OT_INY_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_INY_DETCollection = new[]
                {
                    new
                    {
                        LineId = detalleInyeccion.LineId,
                        U_NROCONTEN = detalleInyeccion.NroContenedor,
                        U_NROMAQUI = detalleInyeccion.NroMaquina,
                        U_FECHAPROC = detalleInyeccion.Fecha,
                        U_TURNO = detalleInyeccion.Turno,
                        U_OPERARIO = detalleInyeccion.Operario,
                        U_CCP1 = detalleInyeccion.PesoColadaKG,
                        U_CCP2 = detalleInyeccion.PesoMasacoteKG,
                        U_CCP3 = detalleInyeccion.PesoAjusMaquinaKG,
                        U_CCP4 = detalleInyeccion.PesoPiezaG,
                        U_CCP5 = detalleInyeccion.CavidadReal,
                        U_CCP6 = detalleInyeccion.CavidadOperativa,
                        U_CCP7 = detalleInyeccion.TiempoCicloReal,
                        U_CCP8 = detalleInyeccion.TiempoCiclo,
                        U_ESTADO = "Pendiente",
                        U_LIBERADO = detalleInyeccion.Liberado ? "Y" : "N",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task RegistrarParada(Parada parada, OTInyeccionDet detalleInyeccion)
        {
            var fechaActual = DateTime.Now;
            var estado = parada.DocEntry == 0 ? "Iniciado" : "Detenido";
            var horaInicio = estado == "Iniciado" ? fechaActual.ToString("HHmm") : parada.HoraInicio.ToString("HHmm");
            var horaFin = estado == "Detenido" ? fechaActual.ToString("HHmm") : parada.HoraFin.ToString("HHmm");

            var method = estado == "Iniciado" ? Method.Post : Method.Patch;
            var entity = estado == "Iniciado" ? $"EEP_PARADAS" : $"EEP_PARADAS({parada.DocEntry})";

            var body = new
            {
                U_ESTACION = "INYECCION",
                U_OT = detalleInyeccion.DocEntry,
                U_LINEIDOT = detalleInyeccion.LineId,
                U_FECHA = parada.Fecha,
                U_TIPOPARO = parada.TipoParada,
                U_TURNO = parada.Turno,
                U_OPERADOR1 = detalleInyeccion.Operario,
                U_OPERADOR2 = "",
                U_HORAINI = horaInicio,
                U_HORAFIN = horaFin,
                U_NROMAQUI = parada.NroMaquina,
                U_ESTADO = estado,
            };

            _connectionService.SetEntitySL(method, entity, body);
        }
    }
}

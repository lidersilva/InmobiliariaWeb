using eProduccion.Data.Configuracion;
using eProduccion.Integration;
using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;

namespace eProduccion.Data.Produccion
{
    public class InyeccionService(ConnectionService connectionService, SBOIntegration sboIntegration, ParametrizacionService parametrizacion)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly SBOIntegration _sboIntegration = sboIntegration;
        private readonly ParametrizacionService _parametrizacion = parametrizacion;

        public Task<OTInyeccion[]> ObtenerOTInyeccion()
        {
            var list = new List<OTInyeccion>();

            var query = $@"
                SELECT 
                TI.""DocEntry"", 
                TI.""U_FECHAOT"", 
                TI.""U_CODARTICULO"", 
                (SELECT x.""ItemName"" FROM ""{_connectionService.DataBase}"".OITM x WHERE x.""ItemCode""=TI.""U_CODARTICULO"") ARTICULO, 
                TI.""U_CODSUBART"", 
                TA.""ItemName"", 
                TI.""U_CANTIDADOT"", 
                TP.""U_DOCNUMOV"", 
                TS.""SeriesName"", 
                TI.""U_ESTADO"", 
                (SELECT IFNULL(x.""U_EP_CPM"", 0) FROM ""{_connectionService.DataBase}"".OITM x WHERE x.""ItemCode""=TI.""U_CODSUBART"") CAVREALES
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_INY_CAB"" TI 
                JOIN ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" TP ON TI.""U_CODEPLANIOT""=TP.""Code"" 
                JOIN ""{_connectionService.DataBase}"".NNM1 TS ON TP.""U_CODSERIE""=TS.""Series"" 
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TI.""U_CODSUBART""=TA.""ItemCode"" 
                ORDER BY TI.""DocEntry"" DESC ";

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
                che.CavidadesReales = int.Parse(reader["CAVREALES"].ToString());
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
                    IFNULL(TS.""DocNum"", 0) ""DocNumSalida"",
                    ""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_INY_DET"" TD
                LEFT JOIN  ""{_connectionService.DataBase}"".OIGE TS ON TD.""U_DESALIDA""=TS.""DocEntry""
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
                che.DocNumSalida = int.Parse(reader["DocNumSalida"].ToString());
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
                        U_CANTAPROB = detalleInyeccion.CantAprobadas,
                        U_CANTRET = detalleInyeccion.CantRetenidas,
                        U_CANTMERMA = detalleInyeccion.CantRechReciclable,
                        U_CANTMERMAKG = detalleInyeccion.PesoRechReciclable,
                        U_CANTMERMA2 = detalleInyeccion.CantRechNoReciclable,
                        U_CANTMERMAKG2 = detalleInyeccion.PesoRechNoReciclable,
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

        public async Task FinalizarLineaInyeccion(string codArticuloOV, string codArticuloI, OTInyeccionDet detalleInyeccion)
        {
            var parametrizacion = await _parametrizacion.ObtenerParametrizacion();

            int cantidadInyeccion = detalleInyeccion.CantAprobadas + detalleInyeccion.CantRetenidas + detalleInyeccion.CantRechReciclable + detalleInyeccion.CantRechNoReciclable;

            var listaMateriales = ObtenerListaMateriales(codArticuloOV, codArticuloI, "02");

            var listaSalidaDet = new List<EntradaSalidaDet>();
            foreach (var i in listaMateriales)
            {
                if (i.TipoItem == 4)
                {
                    var lotes = ObtenerLotesSalida(i.Item, parametrizacion.AlmacenSalidaIny);

                    var che = new EntradaSalidaDet();
                    che.Articulo = i.Item;
                    che.Cantidad = cantidadInyeccion * i.Cantidad;
                    che.LoteDetalle = CalcularLotesAConsumir(che.Cantidad, lotes);
                    listaSalidaDet.Add(che);
                }
            }

            var jObject = _sboIntegration.CrearSalidaMercancias(detalleInyeccion.DocEntry, detalleInyeccion.LineId, listaSalidaDet, parametrizacion.CtaProduccionCurso, parametrizacion.AlmacenSalidaIny, "Inyección");
            int docEntrySalida = int.Parse(jObject["DocEntry"].ToString());

            ActualizarLineaInyeccion(detalleInyeccion.DocEntry, docEntrySalida, detalleInyeccion);
        }

        public List<ListaMaterialesDet> ObtenerListaMateriales(string codArticuloOV, string codArticuloI, string estacion)
        {
            var list = new List<ListaMaterialesDet>();

            var query = $@"
                SELECT 
                    T1.""Code"", 
                    T1.""Quantity"", 
                    T1.""Type"" 
                FROM ""{_connectionService.DataBase}"".OITT T0
                JOIN ""{_connectionService.DataBase}"".ITT1 T1 ON T0.""Code""=T1.""Father""
                JOIN ""{_connectionService.DataBase}"".ITT2 T2 ON T0.""Code""=T2.""Father"" AND T1.""StageId""=T2.""StageId"" 
                JOIN ""{_connectionService.DataBase}"".ORST T3 ON T2.""StgEntry""=T3.""AbsEntry"" 
                WHERE T0.""Code""='{codArticuloOV}'
                AND T3.""Code""='{estacion}'
                AND T2.""U_CodAcabado""='{codArticuloI}' 
                GROUP BY T1.""Code"", T1.""Quantity"", T1.""Type"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new ListaMaterialesDet();
                che.Item = reader["Code"].ToString();
                che.Cantidad = double.Parse(reader["Quantity"].ToString());
                che.TipoItem = int.Parse(reader["Type"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public void ActualizarLineaInyeccion(int docEntryOT, int docEntrySalida, OTInyeccionDet detalleInyeccion)
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
                        U_HORAFIN = fechaActual.ToString("HHmm"),
                        U_CANTAPROB = detalleInyeccion.CantAprobadas,
                        U_CANTRET = detalleInyeccion.CantRetenidas,
                        U_CANTMERMA = detalleInyeccion.CantRechReciclable,
                        U_CANTMERMAKG = detalleInyeccion.PesoRechReciclable,
                        U_CANTMERMA2 = detalleInyeccion.CantRechNoReciclable,
                        U_CANTMERMAKG2 = detalleInyeccion.PesoRechNoReciclable,
                        U_CCP1 = detalleInyeccion.PesoColadaKG,
                        U_CCP2 = detalleInyeccion.PesoMasacoteKG,
                        U_CCP3 = detalleInyeccion.PesoAjusMaquinaKG,
                        U_CCP4 = detalleInyeccion.PesoPiezaG,
                        U_CCP6 = detalleInyeccion.CavidadOperativa,
                        U_CCP7 = detalleInyeccion.TiempoCicloReal,
                        U_CCP8 = detalleInyeccion.TiempoCiclo,
                        U_DESALIDA = docEntrySalida,
                        U_ESTADO = "Finalizado",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public List<Lote> ObtenerLotesSalida(string codArticuloI, string almacen)
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
                GROUP BY TC.""DistNumber"", TD.""Quantity"", TC.""ExpDate""
                ORDER BY TC.""ExpDate"" ";

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

            return list;
        }

        public List<Lote> CalcularLotesAConsumir(double cantidadConsumir, List<Lote> lotes)
        {
            var list = new List<Lote>();

            foreach (var i in lotes)
            {
                if (cantidadConsumir <= 0)
                    break;

                double cantidadConsumida = Math.Min(i.Cantidad, cantidadConsumir);

                list.Add(new Lote()
                {
                    NroLote = i.NroLote,
                    Cantidad = cantidadConsumida,
                });

                cantidadConsumir -= cantidadConsumida;
            }

            return list;
        }
    }
}

using eProduccion.Data.Configuracion;
using eProduccion.Integration;
using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;
using System.Globalization;

namespace eProduccion.Data.Produccion
{
    public class InyeccionExtrusionService(ConnectionService connectionService, SBOIntegration sboIntegration, ParametrizacionService parametrizacion, ProduccionCommonService produccionCommonService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly SBOIntegration _sboIntegration = sboIntegration;
        private readonly ParametrizacionService _parametrizacion = parametrizacion;
        private readonly ProduccionCommonService _produccionCommonService = produccionCommonService;

        public Task<OTInyeccionExtrusion[]> ObtenerOTInyeccionExtrusion(string estacion)
        {
            var list = new List<OTInyeccionExtrusion>();

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
                (SELECT IFNULL(x.""U_EP_CPM"", 0) FROM ""{_connectionService.DataBase}"".OITM x WHERE x.""ItemCode""=TI.""U_CODSUBART"") CAVREALES,
                TI.""U_CODEPLANIOT""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_INYEX_CAB"" TI 
                JOIN ""{_connectionService.DataBase}"".""@EEP_PLANI_OT"" TP ON TI.""U_CODEPLANIOT""=TP.""Code"" 
                JOIN ""{_connectionService.DataBase}"".NNM1 TS ON TP.""U_CODSERIE""=TS.""Series"" 
                JOIN ""{_connectionService.DataBase}"".OITM TA ON TI.""U_CODSUBART""=TA.""ItemCode"" 
                WHERE ""U_ESTACION""='{estacion}'
                ORDER BY TI.""DocEntry"" DESC ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTInyeccionExtrusion();
                che.DocEntry = int.Parse(reader["DocEntry"].ToString());
                che.FechaOT = DateTime.Parse(reader["U_FECHAOT"].ToString());
                che.CodArticuloOV = reader["U_CODARTICULO"].ToString();
                che.ArticuloOV = reader["ARTICULO"].ToString();
                che.CodArticuloIE = reader["U_CODSUBART"].ToString();
                che.ArticuloIE = reader["ItemName"].ToString();
                che.CantidadOT = double.Parse(reader["U_CANTIDADOT"].ToString());
                che.DocNumOV = int.Parse(reader["U_DOCNUMOV"].ToString());
                che.SerieOV = reader["SeriesName"].ToString();
                che.EstadoOT = reader["U_ESTADO"].ToString();
                che.CavidadesReales = int.Parse(reader["CAVREALES"].ToString());
                che.CodePlanificacionOT = int.Parse(reader["U_CODEPLANIOT"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public Task<List<OTInyeccionExtrusionDet>> ObtenerDetalleInyeccionExtrusion(int docEntryOT)
        {
            var list = new List<OTInyeccionExtrusionDet>();

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
                    IFNULL(""U_CCP9"", 0) ""U_CCP9"",
                    IFNULL(""U_CCP10"", 0) ""U_CCP10"",
                    IFNULL(""U_CCP11"", 0) ""U_CCP11"",
                    IFNULL(""U_CCP12"", 0) ""U_CCP12"",
                    ""U_LIBERADO"",
                    IFNULL(TS.""DocNum"", 0) ""DocNumSalida"",
                    IFNULL(TD.""U_NROASIENTO"", 0) ""U_NROASIENTO"",
                    IFNULL(TE.""DocNum"", 0) ""DocNumEntrada"",
                    ""U_ESTADO""
                FROM ""{_connectionService.DataBase}"".""@EEP_OT_INYEX_DET"" TD
                LEFT JOIN  ""{_connectionService.DataBase}"".OIGE TS ON TD.""U_DESALIDA""=TS.""DocEntry""
                LEFT JOIN  ""{_connectionService.DataBase}"".OIGN TE ON TD.""U_DEENTRADA""=TE.""DocEntry""
                WHERE TD.""DocEntry"" = {docEntryOT}
                ORDER BY ""LineId""";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new OTInyeccionExtrusionDet();
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
                che.MetrosMinReal = double.Parse(reader["U_CCP9"].ToString());
                che.MetrosMinuto = double.Parse(reader["U_CCP10"].ToString());
                che.MetrosTurnoReal = double.Parse(reader["U_CCP11"].ToString());
                che.MetrosTurno = double.Parse(reader["U_CCP12"].ToString());
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

        public async Task RegistrarInicioInyeccionExtrusion(int docEntryOT, OTInyeccionExtrusionDet detInyeccionExtrusion)
        {
            var method = Method.Patch;
            var entity = $"EEP_OT_INYEX_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_INYEX_DETCollection = new[]
                {
                    new
                    {
                        LineId = detInyeccionExtrusion.LineId,
                        U_HORAINI = DateTime.Now.ToString("HHmm"),
                        U_ESTADO = "Iniciado",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task GuardarLineaInyeccionExtrusion(int docEntryOT, OTInyeccionExtrusionDet detInyeccionExtrusion)
        {
            var method = Method.Patch;
            var entity = $"EEP_OT_INYEX_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_INYEX_DETCollection = new[]
                {
                    new
                    {
                        LineId = detInyeccionExtrusion.LineId,
                        U_NROCONTEN = detInyeccionExtrusion.NroContenedor,
                        U_NROMAQUI = detInyeccionExtrusion.NroMaquina,
                        U_FECHAPROC = detInyeccionExtrusion.Fecha,
                        U_TURNO = detInyeccionExtrusion.Turno,
                        U_OPERARIO = detInyeccionExtrusion.Operario,
                        U_OPERARIO2 = detInyeccionExtrusion.Operario2,
                        U_CANTAPROB = detInyeccionExtrusion.CantAprobadas,
                        U_CANTRET = detInyeccionExtrusion.CantRetenidas,
                        U_CANTMERMA = detInyeccionExtrusion.CantRechReciclable,
                        U_CANTMERMAKG = detInyeccionExtrusion.PesoRechReciclable,
                        U_MOTIVOMERMA = detInyeccionExtrusion.MotiMPesoRechReciclable,
                        U_CANTMERMA2 = detInyeccionExtrusion.CantRechNoReciclable,
                        U_CANTMERMAKG2 = detInyeccionExtrusion.PesoRechNoReciclable,
                        U_MOTIVOMERMA2 = detInyeccionExtrusion.MotiMPesoRechNoReciclable,
                        U_CCP1 = detInyeccionExtrusion.PesoColadaKG,
                        U_CCP2 = detInyeccionExtrusion.PesoMasacoteKG,
                        U_CCP3 = detInyeccionExtrusion.PesoAjusMaquinaKG,
                        U_CCP4 = detInyeccionExtrusion.PesoPiezaG,
                        U_CCP5 = detInyeccionExtrusion.CavidadReal,
                        U_CCP6 = detInyeccionExtrusion.CavidadOperativa,
                        U_CCP7 = detInyeccionExtrusion.TiempoCicloReal,
                        U_CCP8 = detInyeccionExtrusion.TiempoCiclo,
                        U_CCP9 = detInyeccionExtrusion.MetrosMinReal,
                        U_CCP10 = detInyeccionExtrusion.MetrosMinuto,
                        U_CCP11 = detInyeccionExtrusion.MetrosTurnoReal,
                        U_CCP12 = detInyeccionExtrusion.MetrosTurno,
                        U_ESTADO = "Pendiente",
                        U_LIBERADO = detInyeccionExtrusion.Liberado ? "Y" : "N",
                    }
                }
            };

            _connectionService.SetEntitySL(method, entity, body);
        }

        public async Task FinalizarLineaInyeccionExtrusion(string codArticuloOV, string codArticuloIE, OTInyeccionExtrusionDet detInyeccionExtrusion, string estacion, int codePlanificacionOT)
        {
            var listLineasAsiento = new List<AsientoDet>();
            int nroAsiento = 0;
            double totalDebitoRecurso = 0;

            var parametrizacion = await _parametrizacion.ObtenerParametrizacion();

            string etapaRuta = await _produccionCommonService.ObtenerCodigoEtapaRuta(estacion);
            string comentarioEstacion = estacion == "INYECCION" ? "Inyección" : "Extrusión";
            string almacenSalida = estacion == "INYECCION" ? parametrizacion.AlmacenSalidaIny : parametrizacion.AlmacenSalidaExt;

            int cantidadRecibir = detInyeccionExtrusion.CantAprobadas + detInyeccionExtrusion.CantRetenidas + detInyeccionExtrusion.CantRechReciclable + detInyeccionExtrusion.CantRechNoReciclable;

            var listaMateriales = ObtenerListaMateriales(codArticuloOV, codArticuloIE, etapaRuta);

            // Salida
            var listaSalidaDet = new List<EntradaSalidaDet>();
            foreach (var i in listaMateriales)
            {
                if (i.TipoItem == 4)
                {
                    var lotes = ObtenerLotesSalida(i.Item, almacenSalida);

                    var che = new EntradaSalidaDet();
                    che.Articulo = i.Item;
                    che.Cantidad = cantidadRecibir * i.Cantidad;
                    che.LoteDetalle = CalcularLotesAConsumir(che.Cantidad, lotes);
                    listaSalidaDet.Add(che);
                }
            }

            var jObjectSalida = _sboIntegration.CrearSalidaMercancias(detInyeccionExtrusion.DocEntry, detInyeccionExtrusion.LineId, listaSalidaDet, parametrizacion.CtaProduccionCurso, almacenSalida, comentarioEstacion);
            int docEntrySalida = int.Parse(jObjectSalida["DocEntry"].ToString());
            int docNumSalida = int.Parse(jObjectSalida["DocNum"].ToString());
            int nroAsientoSalida = int.Parse(jObjectSalida["TransNum"].ToString());

            //Asiento
            if (listaMateriales.Any(x => x.TipoItem == 290))
            {
                totalDebitoRecurso = 0;

                // Cuentas de mayor a utilizar en el asiento para los recursos
                var cuentasMayor = ObtenerCuentasMayorRecurso();

                foreach (var i in listaMateriales.Where(x => x.TipoItem == 290))
                {
                    var costosRecurso = ObtenerCostosRecurso(i.Item);

                    var listCostosPorCuenta = ObtenerCostoPorCuentaRecurso(costosRecurso, cuentasMayor);

                    foreach (var j in listCostosPorCuenta)
                    {
                        double creditoRecurso = j.Costo * i.Cantidad * cantidadRecibir;

                        listLineasAsiento.Add(new AsientoDet
                        {
                            AccountCode = j.Cuenta,
                            Credito = creditoRecurso
                        });

                        totalDebitoRecurso += creditoRecurso;
                    }
                }

                listLineasAsiento.Add(new AsientoDet
                {
                    AccountCode = parametrizacion.CtaProduccionCurso,
                    Debito = totalDebitoRecurso
                });

                var jObjectAsiento = _sboIntegration.CrearAsiento(detInyeccionExtrusion.DocEntry, detInyeccionExtrusion.LineId, listLineasAsiento, comentarioEstacion, docNumSalida);
                nroAsiento = int.Parse(jObjectAsiento["JdtNum"].ToString());
            }

            // Entrada
            double totalDebitoSalida = ObtenerDebitoAsientoSalidaMercaderias(nroAsientoSalida);
            double precioUnitarioEntrada = totalDebitoRecurso + totalDebitoSalida / cantidadRecibir;

            var listEntradaDet = new List<EntradaSalidaDet>();
            if (detInyeccionExtrusion.CantAprobadas > 0)
            {
                var almacen = estacion == "INYECCION" ? parametrizacion.AlmacenAprobadosIny : parametrizacion.AlmacenAprobadosExt;

                listEntradaDet.Add(new EntradaSalidaDet
                {
                    Articulo = codArticuloIE,
                    Cantidad = detInyeccionExtrusion.CantAprobadas,
                    Almacen = almacen,
                    PrecioUnitario = precioUnitarioEntrada,
                    LoteDetalle =
                    [
                        new Lote{
                            NroLote = $"{detInyeccionExtrusion.DocEntry}-1-{detInyeccionExtrusion.Turno}-{DateTime.Now:yyyyMMdd}",
                            Cantidad = detInyeccionExtrusion.CantAprobadas,
                        }
                    ]
                });
            }

            if (detInyeccionExtrusion.CantRetenidas > 0)
            {
                var almacen = estacion == "INYECCION" ? parametrizacion.AlmacenRetenidosIny : parametrizacion.AlmacenRetenidosExt;

                listEntradaDet.Add(new EntradaSalidaDet
                {
                    Articulo = codArticuloIE,
                    Cantidad = detInyeccionExtrusion.CantRetenidas,
                    Almacen = almacen,
                    PrecioUnitario = precioUnitarioEntrada,
                    LoteDetalle =
                    [
                        new Lote{
                            NroLote = $"{detInyeccionExtrusion.DocEntry}-1-{detInyeccionExtrusion.Turno}-{DateTime.Now:yyyyMMdd}",
                            Cantidad = detInyeccionExtrusion.CantRetenidas,
                        }
                    ]
                });
            }

            if (detInyeccionExtrusion.CantRechReciclable > 0)
            {
                var almacen = estacion == "INYECCION" ? parametrizacion.AlmacenRechReciIny : parametrizacion.AlmacenRechReciExt;

                listEntradaDet.Add(new EntradaSalidaDet
                {
                    Articulo = codArticuloIE,
                    Cantidad = detInyeccionExtrusion.CantRechReciclable,
                    Almacen = almacen,
                    PrecioUnitario = precioUnitarioEntrada,
                    LoteDetalle =
                    [
                        new Lote{
                            NroLote = $"{detInyeccionExtrusion.DocEntry}-1-{detInyeccionExtrusion.Turno}-{DateTime.Now:yyyyMMdd}",
                            Cantidad = detInyeccionExtrusion.CantRechReciclable,
                        }
                    ]
                });
            }

            if (detInyeccionExtrusion.CantRechNoReciclable > 0)
            {
                var almacen = estacion == "INYECCION" ? parametrizacion.AlmacenRechNoReciIny : parametrizacion.AlmacenRechNoReciExt;

                listEntradaDet.Add(new EntradaSalidaDet
                {
                    Articulo = codArticuloIE,
                    Cantidad = detInyeccionExtrusion.CantRechNoReciclable,
                    Almacen = almacen,
                    PrecioUnitario = precioUnitarioEntrada,
                    LoteDetalle =
                    [
                        new Lote{
                            NroLote = $"{detInyeccionExtrusion.DocEntry}-1-{detInyeccionExtrusion.Turno}-{DateTime.Now:yyyyMMdd}",
                            Cantidad = detInyeccionExtrusion.CantRechNoReciclable,
                        }
                    ]
                });
            }

            var jObjectEntrada = _sboIntegration.CrearEntradaMercancias(detInyeccionExtrusion.DocEntry, detInyeccionExtrusion.LineId, listEntradaDet, parametrizacion.CtaProduccionCurso, comentarioEstacion);
            int docEntryEntrada = int.Parse(jObjectEntrada["DocEntry"].ToString());

            ActualizarLineaInyeccionFinalizacion(detInyeccionExtrusion.DocEntry, docEntrySalida, nroAsiento, docEntryEntrada, detInyeccionExtrusion);

            if (ProcesarEnsamblado(codArticuloOV, codArticuloIE))
            {
                GenerarOrdenPlanifEnsamblado(codePlanificacionOT, detInyeccionExtrusion.CantAprobadas, estacion, detInyeccionExtrusion.DocEntry, detInyeccionExtrusion.LineId, codArticuloIE);
            }

            GenerarOTPendienteMolinar(codePlanificacionOT, detInyeccionExtrusion.CantRechReciclable, estacion, detInyeccionExtrusion.DocEntry, detInyeccionExtrusion.LineId);
        }

        public List<ListaMaterialesDet> ObtenerListaMateriales(string codArticuloOV, string codArticuloI, string estacion)
        {
            var list = new List<ListaMaterialesDet>();

            var query = $@"
                SELECT 
                    T1.""Code"", 
                    T1.""ItemName"", 
                    T1.""Quantity"", 
                    T1.""Type"" 
                FROM ""{_connectionService.DataBase}"".OITT T0
                JOIN ""{_connectionService.DataBase}"".ITT1 T1 ON T0.""Code""=T1.""Father""
                JOIN ""{_connectionService.DataBase}"".ITT2 T2 ON T0.""Code""=T2.""Father"" AND T1.""StageId""=T2.""StageId"" 
                JOIN ""{_connectionService.DataBase}"".ORST T3 ON T2.""StgEntry""=T3.""AbsEntry"" 
                WHERE T0.""Code""='{codArticuloOV}'
                AND T3.""Code""='{estacion}'
                AND T2.""U_CodAcabado""='{codArticuloI}' 
                GROUP BY T1.""Code"", T1.""ItemName"", T1.""Quantity"", T1.""Type"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new ListaMaterialesDet();
                che.Item = reader["Code"].ToString();
                che.Descripcion = reader["ItemName"].ToString();
                che.Cantidad = double.Parse(reader["Quantity"].ToString());
                che.TipoItem = int.Parse(reader["Type"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public void ActualizarLineaInyeccionFinalizacion(int docEntryOT, int docEntrySalida, int nroAsiento, int docEntryEntrada, OTInyeccionExtrusionDet detInyeccionExtrusion)
        {
            var fechaActual = DateTime.Now;

            var method = Method.Patch;
            var entity = $"EEP_OT_INYEX_CAB({docEntryOT})";

            var body = new
            {
                EEP_OT_INYEX_DETCollection = new[]
                {
                    new
                    {
                        LineId = detInyeccionExtrusion.LineId,
                        U_HORAFIN = fechaActual.ToString("HHmm"),
                        U_CANTAPROB = detInyeccionExtrusion.CantAprobadas,
                        U_CANTRET = detInyeccionExtrusion.CantRetenidas,
                        U_CANTMERMA = detInyeccionExtrusion.CantRechReciclable,
                        U_CANTMERMAKG = detInyeccionExtrusion.PesoRechReciclable,
                        U_MOTIVOMERMA = detInyeccionExtrusion.MotiMPesoRechReciclable,
                        U_CANTMERMA2 = detInyeccionExtrusion.CantRechNoReciclable,
                        U_CANTMERMAKG2 = detInyeccionExtrusion.PesoRechNoReciclable,
                        U_MOTIVOMERMA2 = detInyeccionExtrusion.MotiMPesoRechNoReciclable,
                        U_CCP1 = detInyeccionExtrusion.PesoColadaKG,
                        U_CCP2 = detInyeccionExtrusion.PesoMasacoteKG,
                        U_CCP3 = detInyeccionExtrusion.PesoAjusMaquinaKG,
                        U_CCP4 = detInyeccionExtrusion.PesoPiezaG,
                        U_CCP6 = detInyeccionExtrusion.CavidadOperativa,
                        U_CCP7 = detInyeccionExtrusion.TiempoCicloReal,
                        U_CCP8 = detInyeccionExtrusion.TiempoCiclo,
                        U_CCP9 = detInyeccionExtrusion.MetrosMinReal,
                        U_CCP10 = detInyeccionExtrusion.MetrosMinuto,
                        U_CCP11 = detInyeccionExtrusion.MetrosTurnoReal,
                        U_CCP12 = detInyeccionExtrusion.MetrosTurno,
                        U_DESALIDA = docEntrySalida,
                        U_NROASIENTO = nroAsiento,
                        U_DEENTRADA = docEntryEntrada,
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
                ORDER BY IFNULL(TC.""ExpDate"", CURRENT_DATE) ";

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

        public CostoRecurso ObtenerCostosRecurso(string codRecurso)
        {
            var mCostoRecurso = new CostoRecurso();

            var query = $@"
                SELECT 
                ""StdCost1"",
                ""StdCost2"",
                ""StdCost3"",
                ""StdCost4"",
                ""StdCost5"",
                ""StdCost6"",
                ""StdCost7"",
                ""StdCost8"",
                ""StdCost9"",
                ""StdCost10""
                FROM ""{_connectionService.DataBase}"".ORSC 
                WHERE ""VisResCode""='{codRecurso}' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                mCostoRecurso.CostoRecurso1 = double.Parse(reader["StdCost1"].ToString());
                mCostoRecurso.CostoRecurso2 = double.Parse(reader["StdCost2"].ToString());
                mCostoRecurso.CostoRecurso3 = double.Parse(reader["StdCost3"].ToString());
                mCostoRecurso.CostoRecurso4 = double.Parse(reader["StdCost4"].ToString());
                mCostoRecurso.CostoRecurso5 = double.Parse(reader["StdCost5"].ToString());
                mCostoRecurso.CostoRecurso6 = double.Parse(reader["StdCost6"].ToString());
                mCostoRecurso.CostoRecurso7 = double.Parse(reader["StdCost7"].ToString());
                mCostoRecurso.CostoRecurso8 = double.Parse(reader["StdCost8"].ToString());
                mCostoRecurso.CostoRecurso9 = double.Parse(reader["StdCost9"].ToString());
                mCostoRecurso.CostoRecurso10 = double.Parse(reader["StdCost10"].ToString());
            }

            _connectionService.DisconnectODBC();

            return mCostoRecurso;
        }

        public CuentaMayorRecurso ObtenerCuentasMayorRecurso()
        {
            var mCCuentaMayorRecurso = new CuentaMayorRecurso();

            var query = $@"
                SELECT 
                ""ResStdExp1"", 
                ""ResStdExp2"",
                ""ResStdExp3"",
                ""ResStdExp4"",
                ""ResStdExp5"",
                ""ResStdExp6"",
                ""ResStdExp7"",
                ""ResStdExp8"",
                ""ResStdExp9"",
                ""ResStdEx10"" 
                FROM ""{_connectionService.DataBase}"".OACP
                WHERE ""PeriodCat""={DateTime.Now.Year} ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                mCCuentaMayorRecurso.GastoCosto1 = reader["ResStdExp1"].ToString();
                mCCuentaMayorRecurso.GastoCosto2 = reader["ResStdExp2"].ToString();
                mCCuentaMayorRecurso.GastoCosto3 = reader["ResStdExp3"].ToString();
                mCCuentaMayorRecurso.GastoCosto4 = reader["ResStdExp4"].ToString();
                mCCuentaMayorRecurso.GastoCosto5 = reader["ResStdExp5"].ToString();
                mCCuentaMayorRecurso.GastoCosto6 = reader["ResStdExp6"].ToString();
                mCCuentaMayorRecurso.GastoCosto7 = reader["ResStdExp7"].ToString();
                mCCuentaMayorRecurso.GastoCosto8 = reader["ResStdExp8"].ToString();
                mCCuentaMayorRecurso.GastoCosto9 = reader["ResStdExp9"].ToString();
                mCCuentaMayorRecurso.GastoCosto10 = reader["ResStdEx10"].ToString();
            }

            _connectionService.DisconnectODBC();

            return mCCuentaMayorRecurso;
        }

        public List<CostoCuentaRecurso> ObtenerCostoPorCuentaRecurso(CostoRecurso costos, CuentaMayorRecurso cuentas)
        {
            var listCostoCuenta = new List<CostoCuentaRecurso>();

            for (int i = 1; i <= 10; i++)
            {
                var propCosto = typeof(CostoRecurso).GetProperty($"CostoRecurso{i}");
                var propCuenta = typeof(CuentaMayorRecurso).GetProperty($"GastoCosto{i}");

                if (propCosto != null && propCuenta != null)
                {
                    var valorCosto = (double)(propCosto.GetValue(costos) ?? 0);
                    var valorCuenta = (string)(propCuenta.GetValue(cuentas) ?? string.Empty);

                    if (valorCosto > 0)
                    {
                        listCostoCuenta.Add(new CostoCuentaRecurso
                        {
                            Numero = i,
                            Costo = valorCosto,
                            Cuenta = valorCuenta,
                        });
                    }
                }
            }

            return listCostoCuenta;
        }

        public double ObtenerDebitoAsientoSalidaMercaderias(int asiento)
        {
            double totalDebito = 0;

            var query = $@"
                SELECT SUM(""Debit"") ""Debit"" FROM ""{_connectionService.DataBase}"".JDT1 WHERE ""TransId""={asiento} ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                totalDebito = double.Parse(reader["Debit"].ToString());
            }

            _connectionService.DisconnectODBC();

            return totalDebito;
        }

        public async Task<List<MotivoParadaDefecto>> ObtenerMotivosParadaDefecto()
        {
            var listMotivos = new List<MotivoParadaDefecto>();

            var query = $@"SELECT ""U_CODIGO"", ""U_DESCRIPCION"", ""U_TIPO"" FROM ""{_connectionService.DataBase}"".""@EEP_PARADA_DEFECTO"" ORDER BY ""U_CODIGO"" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new MotivoParadaDefecto();
                che.Codigo = reader["U_CODIGO"].ToString();
                che.Descripcion = reader["U_DESCRIPCION"].ToString();
                che.Tipo = reader["U_TIPO"].ToString();
                listMotivos.Add(che);
            }

            _connectionService.DisconnectODBC();

            return listMotivos;
        }

        private bool ProcesarEnsamblado(string codArticuloOV, string codArticuloI)
        {
            var conEnsamblado = false;

            var query = $@"
                SELECT 
                T0.""SeqNum""
                FROM ""{_connectionService.DataBase}"".ITT2 T0 
                WHERE T0.""Father""='{codArticuloOV}' 
                AND T0.""SeqNum"" = (
                        SELECT MAX(x.""SeqNum"") + 1
                        FROM ""{_connectionService.DataBase}"".ITT2 x
                        WHERE x.""Father"" = T0.""Father""
                          AND x.""U_CodAcabado"" = '{codArticuloI}'
                        )";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                conEnsamblado = true;
            }

            _connectionService.DisconnectODBC();

            return conEnsamblado;
        }

        private int ObtenerPlanifEnsamblado(int codePlanificacionOT, string estacion)
        {
            var OTEnsamblado = 0;

            var query = $@"SELECT ""DocEntry"" FROM ""{_connectionService.DataBase}"".""@EEP_ENSAM_CAB"" WHERE ""U_CODEPLANIOT""={codePlanificacionOT} AND ""U_ESTANTERIOR""='{estacion}' ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                OTEnsamblado = int.Parse(reader["DocEntry"].ToString()); ;
            }

            _connectionService.DisconnectODBC();

            return OTEnsamblado;
        }

        private void GenerarOrdenPlanifEnsamblado(int codePlanificacionOT, int cantAprobadas, string estacion, int docEntryOT, int lineIdOT, string codArticuloIE)
        {
            var otEnsamblado = ObtenerPlanifEnsamblado(codePlanificacionOT, estacion);

            var method = otEnsamblado != 0 ? Method.Patch : Method.Post;
            var entity = otEnsamblado != 0 ? $"EEP_ENSAM_CAB({otEnsamblado})" : $"EEP_ENSAM_CAB";
            dynamic body = null;

            if (otEnsamblado == 0)
            {
                body = new
                {
                    U_CODEPLANIOT = codePlanificacionOT,
                    U_ESTANTERIOR = estacion,
                    EEP_ENSAM_DETCollection = new[]
                    {
                        new
                        {
                            U_CANTPROD = cantAprobadas,
                            U_CANTPRODKG = 0,
                            U_CANTSOLICITADA = 0,
                            U_OT = docEntryOT,
                            U_LINEIDOT = lineIdOT,
                            U_CODSUBART = codArticuloIE,
                            U_ESTADO = "Pendiente",
                        }
                    }
                };
            }
            else
            {
                body = new
                {
                    EEP_ENSAM_DETCollection = new[]
                    {
                        new
                        {
                            U_CANTPROD = cantAprobadas,
                            U_CANTPRODKG = 0,
                            U_CANTSOLICITADA = 0,
                            U_OT = docEntryOT,
                            U_LINEIDOT = lineIdOT,
                            U_CODSUBART = codArticuloIE,
                            U_ESTADO = "Pendiente",
                        }
                    }
                };
            }

            _connectionService.SetEntitySL(method, entity, body);
        }

        private void GenerarOTPendienteMolinar(int codePlanificacionOT, int cantRechazadaReciclable, string estacion, int docEntryOT, int lineIdOT)
        {
            var method = Method.Post;
            var entity = $"EEP_PEND_MOLI_OT";

            var body = new
            {
                U_CODEPLANIOT = codePlanificacionOT,
                U_ESTANTERIOR = estacion,
                U_OT = docEntryOT,
                U_LINEIDOT = lineIdOT,
                U_CANTPROD = cantRechazadaReciclable,
                U_CANTSOLICITADA = 0,
            };

            _connectionService.SetEntitySL(method, entity, body);
        }
    }
}

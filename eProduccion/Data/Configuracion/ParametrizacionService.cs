using eProduccion.Models;
using RestSharp;
using System.Data.Odbc;

namespace eProduccion.Data.Configuracion
{
    public class ParametrizacionService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        public async Task<Parametrizacion> ObtenerParametrizacion()
        {
            var parametrizacion = new Parametrizacion();
            var listSeriesDet = new List<SerieDetalle>();
            bool banderaCabecera = true;

            var query = $"SELECT \n" +
                $"TP.\"DocEntry\", \n" +
                $"TP.\"U_CTAPRODC\", \n" +

                // === Códigos de estaciones de trabajo ===
                $"TP.\"U_CINYECCION\", \n" +
                $"TP.\"U_CEXTRUSION\", \n" +
                $"TP.\"U_CARMADO\", \n" +
                $"TP.\"U_CFLOWPACK\", \n" +
                $"TP.\"U_CSACHETERA\", \n" +
                $"TP.\"U_CSELLADO\", \n" +
                $"TP.\"U_CHORNEADO\", \n" +
                $"TP.\"U_CEMPAQUE\", \n" +
                $"TP.\"U_CPRENSA\", \n" +
                $"TP.\"U_CGRABADOL\", \n" +

                $"TP.\"U_ASALIDAI\", \n" +
                $"TP.\"U_AAPROBI\", \n" +
                $"TP.\"U_ARECHRECII\", \n" +
                $"TP.\"U_ARECHNORECII\", \n" +
                $"TP.\"U_ARETENIDOSI\", \n" +

                $"TP.\"U_ASALIDAE\", \n" +
                $"TP.\"U_AAPROBE\", \n" +
                $"TP.\"U_ARECHRECIE\", \n" +
                $"TP.\"U_ARECHNORECIE\", \n" +
                $"TP.\"U_ARETENIDOSE\", \n" +

                $"TP.\"U_AAPROBAR\", \n" +   // Alm. aprobados armado
                $"TP.\"U_AAPROBFL\", \n" +   // Alm. aprobados flowpack
                $"TP.\"U_AAPROBSA\", \n" +   // Alm. aprobados sachetera
                $"TP.\"U_AAPROBSE\", \n" +   // Alm. aprobados sellado
                $"TP.\"U_AAPROBHO\", \n" +   // Alm. aprobados horneado
                $"TP.\"U_AAPROBEM\", \n" +   // Alm. aprobados empaquetado
                $"TP.\"U_AAPROBPR\", \n" +   // Alm. aprobados prensa
                $"TP.\"U_AAPROBGL\", \n" +   // Alm. aprobados grabado láser

                $"TP.\"U_ARECHAR\", \n" +    // Alm. rechazados armado
                $"TP.\"U_ARECHFL\", \n" +    // Alm. rechazados flowpack
                $"TP.\"U_ARECHSA\", \n" +    // Alm. rechazados sachetera
                $"TP.\"U_ARECHSE\", \n" +    // Alm. rechazados sellado
                $"TP.\"U_ARECHHO\", \n" +    // Alm. rechazados horneado
                $"TP.\"U_ARECHEM\", \n" +    // Alm. rechazados empaquetado
                $"TP.\"U_ARECHPR\", \n" +    // Alm. rechazados prensa
                $"TP.\"U_ARECHGL\", \n" +    // Alm. rechazados grabado láser

                $"TP.\"U_ARECUAR\", \n" +    // Alm. recuperados armado
                $"TP.\"U_ARECUFL\", \n" +    // Alm. recuperados flowpack
                $"TP.\"U_ARECUSA\", \n" +    // Alm. recuperados sachetera
                $"TP.\"U_ARECUSE\", \n" +    // Alm. recuperados sellado
                $"TP.\"U_ARECUHO\", \n" +    // Alm. recuperados horneado
                $"TP.\"U_ARECUEM\", \n" +    // Alm. recuperados empaquetado
                $"TP.\"U_ARECUPR\", \n" +    // Alm. recuperados prensa
                $"TP.\"U_ARECUGL\", \n" +    // Alm. recuperados grabado láser

                $"TP.\"U_ARETEAR\", \n" +    // Alm. retenidos armado
                $"TP.\"U_ARETEFL\", \n" +    // Alm. retenidos flowpack
                $"TP.\"U_ARETESA\", \n" +    // Alm. retenidos sachetera
                $"TP.\"U_ARETESE\", \n" +    // Alm. retenidos sellado
                $"TP.\"U_ARETEHO\", \n" +    // Alm. retenidos horneado
                $"TP.\"U_ARETEEM\", \n" +    // Alm. retenidos empaquetado
                $"TP.\"U_ARETEPR\", \n" +    // Alm. retenidos prensa
                $"TP.\"U_ARETEGL\", \n" +    // Alm. retenidos grabado láser

                $"TP.\"U_AREPROAR\", \n" +   // Alm. reprocesados armado
                $"TP.\"U_AREPROFL\", \n" +   // Alm. reprocesados flowpack
                $"TP.\"U_AREPROSA\", \n" +   // Alm. reprocesados sachetera
                $"TP.\"U_AREPROSE\", \n" +   // Alm. reprocesados sellado
                $"TP.\"U_AREPROHO\", \n" +   // Alm. reprocesados horneado
                $"TP.\"U_AREPROEM\", \n" +   // Alm. reprocesados empaquetado
                $"TP.\"U_AREPROPR\", \n" +   // Alm. reprocesados prensa
                $"TP.\"U_AREPROGL\", \n" +   // Alm. reprocesados grabado láser

                $"TP.\"U_ARECIMOLI\", \n" +     // Alm. reciclables molino
                $"TP.\"U_ANORECIMOLI\", \n" +   // Alm. no reciclables molino

                $"TDS.\"LineId\", \n" +
                $"TDS.\"U_CODSERIE\", \n" +
                $"TDS.\"U_SERIE\" " +
                $"FROM \"{_connectionService.DataBase}\".\"@EEP_PARAM\" TP \n" +
                $"LEFT JOIN \"{_connectionService.DataBase}\".\"@EEP_PARSERIE_DET\" TDS ON TP.\"DocEntry\"=TDS.\"DocEntry\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (banderaCabecera)
                {
                    parametrizacion.DocEntry = int.Parse(reader["DocEntry"].ToString());
                    parametrizacion.CtaProduccionCurso = reader["U_CTAPRODC"].ToString();

                    // === Códigos de estaciones de trabajo ===
                    parametrizacion.CodEstacionInyeccion = reader["U_CINYECCION"].ToString();
                    parametrizacion.CodEstacionExtrusion = reader["U_CEXTRUSION"].ToString();
                    parametrizacion.CodEstacionArmado = reader["U_CARMADO"].ToString();
                    parametrizacion.CodEstacionFlowpack = reader["U_CFLOWPACK"].ToString();
                    parametrizacion.CodEstacionSachetera = reader["U_CSACHETERA"].ToString();
                    parametrizacion.CodEstacionSellado = reader["U_CSELLADO"].ToString();
                    parametrizacion.CodEstacionHorneado = reader["U_CHORNEADO"].ToString();
                    parametrizacion.CodEstacionEmpaquetado = reader["U_CEMPAQUE"].ToString();
                    parametrizacion.CodEstacionPrensa = reader["U_CPRENSA"].ToString();
                    parametrizacion.CodEstacionGrabadoL = reader["U_CGRABADOL"].ToString();

                    // ==================== INYECCIÓN ====================
                    parametrizacion.AlmacenSalidaIny = reader["U_ASALIDAI"].ToString();
                    parametrizacion.AlmacenAprobadosIny = reader["U_AAPROBI"].ToString();
                    parametrizacion.AlmacenRechReciIny = reader["U_ARECHRECII"].ToString();
                    parametrizacion.AlmacenRechNoReciIny = reader["U_ARECHNORECII"].ToString();
                    parametrizacion.AlmacenRetenidosIny = reader["U_ARETENIDOSI"].ToString();

                    // ==================== EXTRUSIÓN ====================
                    parametrizacion.AlmacenSalidaExt = reader["U_ASALIDAE"].ToString();
                    parametrizacion.AlmacenAprobadosExt = reader["U_AAPROBE"].ToString();
                    parametrizacion.AlmacenRechReciExt = reader["U_ARECHRECIE"].ToString();
                    parametrizacion.AlmacenRechNoReciExt = reader["U_ARECHNORECIE"].ToString();
                    parametrizacion.AlmacenRetenidosExt = reader["U_ARETENIDOSE"].ToString();

                    // ==================== ARMADO ====================
                    parametrizacion.AlmacenAprobadosArmado = reader["U_AAPROBAR"].ToString();
                    parametrizacion.AlmacenRechazadoArmado = reader["U_ARECHAR"].ToString();
                    parametrizacion.AlmacenRecuperadoArmado = reader["U_ARECUAR"].ToString();
                    parametrizacion.AlmacenRetenidoArmado = reader["U_ARETEAR"].ToString();
                    parametrizacion.AlmacenReprocesoArmado = reader["U_AREPROAR"].ToString();

                    // ==================== FLOWPACK ====================
                    parametrizacion.AlmacenAprobadosFlowpack = reader["U_AAPROBFL"].ToString();
                    parametrizacion.AlmacenRechazadoFlowpack = reader["U_ARECHFL"].ToString();
                    parametrizacion.AlmacenRecuperadoFlowpack = reader["U_ARECUFL"].ToString();
                    parametrizacion.AlmacenRetenidoFlowpack = reader["U_ARETEFL"].ToString();
                    parametrizacion.AlmacenReprocesoFlowpack = reader["U_AREPROFL"].ToString();

                    // ==================== SACHETERA ====================
                    parametrizacion.AlmacenAprobadosSachetera = reader["U_AAPROBSA"].ToString();
                    parametrizacion.AlmacenRechazadoSachetera = reader["U_ARECHSA"].ToString();
                    parametrizacion.AlmacenRecuperadoSachetera = reader["U_ARECUSA"].ToString();
                    parametrizacion.AlmacenRetenidoSachetera = reader["U_ARETESA"].ToString();
                    parametrizacion.AlmacenReprocesoSachetera = reader["U_AREPROSA"].ToString();

                    // ==================== SELLADO ====================
                    parametrizacion.AlmacenAprobadosSellado = reader["U_AAPROBSE"].ToString();
                    parametrizacion.AlmacenRechazadoSellado = reader["U_ARECHSE"].ToString();
                    parametrizacion.AlmacenRecuperadoSellado = reader["U_ARECUSE"].ToString();
                    parametrizacion.AlmacenRetenidoSellado = reader["U_ARETESE"].ToString();
                    parametrizacion.AlmacenReprocesoSellado = reader["U_AREPROSE"].ToString();

                    // ==================== HORNEADO ====================
                    parametrizacion.AlmacenAprobadosHorno = reader["U_AAPROBHO"].ToString();
                    parametrizacion.AlmacenRechazadoHorno = reader["U_ARECHHO"].ToString();
                    parametrizacion.AlmacenRecuperadoHorno = reader["U_ARECUHO"].ToString();
                    parametrizacion.AlmacenRetenidoHorno = reader["U_ARETEHO"].ToString();
                    parametrizacion.AlmacenReprocesoHorno = reader["U_AREPROHO"].ToString();

                    // ==================== EMPAQUETADO ====================
                    parametrizacion.AlmacenAprobadosEmpaque = reader["U_AAPROBEM"].ToString();
                    parametrizacion.AlmacenRechazadoEmpaque = reader["U_ARECHEM"].ToString();
                    parametrizacion.AlmacenRecuperadoEmpaque = reader["U_ARECUEM"].ToString();
                    parametrizacion.AlmacenRetenidoEmpaque = reader["U_ARETEEM"].ToString();
                    parametrizacion.AlmacenReprocesoEmpaque = reader["U_AREPROEM"].ToString();

                    // ==================== PRENSA ====================
                    parametrizacion.AlmacenAprobadosPrensa = reader["U_AAPROBPR"].ToString();
                    parametrizacion.AlmacenRechazadoPrensa = reader["U_ARECHPR"].ToString();
                    parametrizacion.AlmacenRecuperadoPrensa = reader["U_ARECUPR"].ToString();
                    parametrizacion.AlmacenRetenidoPrensa = reader["U_ARETEPR"].ToString();
                    parametrizacion.AlmacenReprocesoPrensa = reader["U_AREPROPR"].ToString();

                    // ==================== GRABADO L ====================
                    parametrizacion.AlmacenAprobadosGrabadoL = reader["U_AAPROBGL"].ToString();
                    parametrizacion.AlmacenRechazadoGrabadoL = reader["U_ARECHGL"].ToString();
                    parametrizacion.AlmacenRecuperadoGrabadoL = reader["U_ARECUGL"].ToString();
                    parametrizacion.AlmacenRetenidoGrabadoL = reader["U_ARETEGL"].ToString();
                    parametrizacion.AlmacenReprocesoGrabadoL = reader["U_AREPROGL"].ToString();

                    // ==================== MOLINO ====================
                    parametrizacion.AlmacenReciMolino = reader["U_ARECIMOLI"].ToString();
                    parametrizacion.AlmacenNoReciMolino = reader["U_ANORECIMOLI"].ToString();
                }

                var serieDet = new SerieDetalle();
                serieDet.LineId = int.Parse(reader["LineId"].ToString());
                serieDet.CodigoSerie = int.Parse(reader["U_CODSERIE"].ToString());
                serieDet.Serie = reader["U_SERIE"].ToString();
                listSeriesDet.Add(serieDet);
            }

            parametrizacion.SerieDetalle = listSeriesDet;

            _connectionService.DisconnectODBC();

            return await Task.FromResult(parametrizacion);
        }

        public Task<Serie[]> ObtenerSeriesOV()
        {
            var list = new List<Serie>();

            var query = $"SELECT \"Series\", \"SeriesName\" FROM \"{_connectionService.DataBase}\".NNM1 WHERE \"ObjectCode\"='17' ORDER BY \"SeriesName\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Serie();
                che.CodigoSerie = int.Parse(reader["Series"].ToString());
                che.Descripcion = reader["SeriesName"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list.ToArray());
        }

        public List<SerieDetalle> ObtenerSeriesParametrizacion()
        {
            var list = new List<SerieDetalle>();

            var query = $"SELECT \"U_CODSERIE\" FROM \"{_connectionService.DataBase}\".\"@EEP_PARSERIE_DET\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new SerieDetalle();
                che.CodigoSerie = int.Parse(reader["U_CODSERIE"].ToString());
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return list;
        }

        public Task<List<CuentaContable>> ObtenerCuentasContables()
        {
            var list = new List<CuentaContable>();

            var query = $"SELECT \"AcctCode\", \"AcctName\" From \"{_connectionService.DataBase}\".OACT WHERE \"Postable\"='Y' AND \"Levels\"=5 ORDER BY \"AcctCode\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new CuentaContable();
                che.Cuenta = reader["AcctCode"].ToString();
                che.Descripcion = reader["AcctName"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public Task<List<EtapaRuta>> ObtenerEtapasRuta()
        {
            var list = new List<EtapaRuta>();

            var query = $"SELECT \"Code\", \"Desc\" FROM \"{_connectionService.DataBase}\".ORST ORDER BY \"Desc\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new EtapaRuta();
                che.Codigo = reader["Code"].ToString();
                che.Descripcion = reader["Desc"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public Task<List<Almacen>> ObtenerAlmacenes()
        {
            var list = new List<Almacen>();

            var query = $"SELECT \"WhsCode\", \"WhsName\" FROM \"{_connectionService.DataBase}\".\"OWHS\" ORDER BY \"WhsCode\" ";

            var command = new OdbcCommand(query, _connectionService.ConnectODBC());
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var che = new Almacen();
                che.Codigo = reader["WhsCode"].ToString();
                che.Descripcion = reader["WhsName"].ToString();
                list.Add(che);
            }

            _connectionService.DisconnectODBC();

            return Task.FromResult(list);
        }

        public async Task GuardarParametrizacion(Parametrizacion parametrizacion)
        {
            var method = parametrizacion.DocEntry == null
                    ? Method.Post : Method.Patch;

            var entity = method == Method.Post
                ? $"EEP_PARAM" : $"EEP_PARAM({parametrizacion.DocEntry})";

            var listSeriesDet = new List<dynamic>();
            foreach (var item in parametrizacion.SerieDetalle)
            {
                var bodyDet = new
                {
                    U_CODSERIE = item.CodigoSerie,
                    U_SERIE = item.Serie,
                };

                listSeriesDet.Add(bodyDet);
            }

            var body = new
            {
                U_CTAPRODC = parametrizacion.CtaProduccionCurso,

                // === Códigos de estaciones de trabajo ===
                U_CINYECCION = parametrizacion.CodEstacionInyeccion,
                U_CEXTRUSION = parametrizacion.CodEstacionExtrusion,
                U_CARMADO = parametrizacion.CodEstacionArmado,
                U_CFLOWPACK = parametrizacion.CodEstacionFlowpack,
                U_CSACHETERA = parametrizacion.CodEstacionSachetera,
                U_CSELLADO = parametrizacion.CodEstacionSellado,
                U_CHORNEADO = parametrizacion.CodEstacionHorneado,
                U_CEMPAQUE = parametrizacion.CodEstacionEmpaquetado,
                U_CPRENSA = parametrizacion.CodEstacionPrensa,
                U_CGRABADOL = parametrizacion.CodEstacionGrabadoL,

                // ==================== INYECCIÓN ====================
                U_ASALIDAI = parametrizacion.AlmacenSalidaIny,
                U_AAPROBI = parametrizacion.AlmacenAprobadosIny,
                U_ARECHRECII = parametrizacion.AlmacenRechReciIny,
                U_ARECHNORECII = parametrizacion.AlmacenRechNoReciIny,
                U_ARETENIDOSI = parametrizacion.AlmacenRetenidosIny,

                // ==================== EXTRUSIÓN ====================
                U_ASALIDAE = parametrizacion.AlmacenSalidaExt,
                U_AAPROBE = parametrizacion.AlmacenAprobadosExt,
                U_ARECHRECIE = parametrizacion.AlmacenRechReciExt,
                U_ARECHNORECIE = parametrizacion.AlmacenRechNoReciExt,
                U_ARETENIDOSE = parametrizacion.AlmacenRetenidosExt,

                // ==================== ARMADO ====================
                U_AAPROBAR = parametrizacion.AlmacenAprobadosArmado,
                U_ARECHAR = parametrizacion.AlmacenRechazadoArmado,
                U_ARECUAR = parametrizacion.AlmacenRecuperadoArmado,
                U_ARETEAR = parametrizacion.AlmacenRetenidoArmado,
                U_AREPROAR = parametrizacion.AlmacenReprocesoArmado,

                // ==================== FLOWPACK ====================
                U_AAPROBFL = parametrizacion.AlmacenAprobadosFlowpack,
                U_ARECHFL = parametrizacion.AlmacenRechazadoFlowpack,
                U_ARECUFL = parametrizacion.AlmacenRecuperadoFlowpack,
                U_ARETEFL = parametrizacion.AlmacenRetenidoFlowpack,
                U_AREPROFL = parametrizacion.AlmacenReprocesoFlowpack,

                // ==================== SACHETERA ====================
                U_AAPROBSA = parametrizacion.AlmacenAprobadosSachetera,
                U_ARECHSA = parametrizacion.AlmacenRechazadoSachetera,
                U_ARECUSA = parametrizacion.AlmacenRecuperadoSachetera,
                U_ARETESA = parametrizacion.AlmacenRetenidoSachetera,
                U_AREPROSA = parametrizacion.AlmacenReprocesoSachetera,

                // ==================== SELLADO ====================
                U_AAPROBSE = parametrizacion.AlmacenAprobadosSellado,
                U_ARECHSE = parametrizacion.AlmacenRechazadoSellado,
                U_ARECUSE = parametrizacion.AlmacenRecuperadoSellado,
                U_ARETESE = parametrizacion.AlmacenRetenidoSellado,
                U_AREPROSE = parametrizacion.AlmacenReprocesoSellado,

                // ==================== HORNEADO ====================
                U_AAPROBHO = parametrizacion.AlmacenAprobadosHorno,
                U_ARECHHO = parametrizacion.AlmacenRechazadoHorno,
                U_ARECUHO = parametrizacion.AlmacenRecuperadoHorno,
                U_ARETEHO = parametrizacion.AlmacenRetenidoHorno,
                U_AREPROHO = parametrizacion.AlmacenReprocesoHorno,

                // ==================== EMPAQUETADO ====================
                U_AAPROBEM = parametrizacion.AlmacenAprobadosEmpaque,
                U_ARECHEM = parametrizacion.AlmacenRechazadoEmpaque,
                U_ARECUEM = parametrizacion.AlmacenRecuperadoEmpaque,
                U_ARETEEM = parametrizacion.AlmacenRetenidoEmpaque,
                U_AREPROEM = parametrizacion.AlmacenReprocesoEmpaque,

                // ==================== PRENSA ====================
                U_AAPROBPR = parametrizacion.AlmacenAprobadosPrensa,
                U_ARECHPR = parametrizacion.AlmacenRechazadoPrensa,
                U_ARECUPR = parametrizacion.AlmacenRecuperadoPrensa,
                U_ARETEPR = parametrizacion.AlmacenRetenidoPrensa,
                U_AREPROPR = parametrizacion.AlmacenReprocesoPrensa,

                // ==================== GRABADO L ====================
                U_AAPROBGL = parametrizacion.AlmacenAprobadosGrabadoL,
                U_ARECHGL = parametrizacion.AlmacenRechazadoGrabadoL,
                U_ARECUGL = parametrizacion.AlmacenRecuperadoGrabadoL,
                U_ARETEGL = parametrizacion.AlmacenRetenidoGrabadoL,
                U_AREPROGL = parametrizacion.AlmacenReprocesoGrabadoL,

                // ==================== MOLINO ====================
                U_ARECIMOLI = parametrizacion.AlmacenReciMolino,
                U_ANORECIMOLI = parametrizacion.AlmacenNoReciMolino,

                EEP_PARSERIE_DETCollection = listSeriesDet
            };

            _connectionService.SetEntitySL(method, entity, body, true);
        }
    }
}

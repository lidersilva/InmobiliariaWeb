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
                    parametrizacion.AlmacenSalidaIny = reader["U_ASALIDAI"].ToString();
                    parametrizacion.AlmacenAprobadosIny = reader["U_AAPROBI"].ToString();
                    parametrizacion.AlmacenRechReciIny = reader["U_ARECHRECII"].ToString();
                    parametrizacion.AlmacenRechNoReciIny = reader["U_ARECHNORECII"].ToString();
                    parametrizacion.AlmacenRetenidosIny = reader["U_ARETENIDOSI"].ToString();
                    parametrizacion.AlmacenSalidaExt = reader["U_ASALIDAE"].ToString();
                    parametrizacion.AlmacenAprobadosExt = reader["U_AAPROBE"].ToString();
                    parametrizacion.AlmacenRechReciExt = reader["U_ARECHRECIE"].ToString();
                    parametrizacion.AlmacenRechNoReciExt = reader["U_ARECHNORECIE"].ToString();
                    parametrizacion.AlmacenRetenidosExt = reader["U_ARETENIDOSE"].ToString();
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
                U_ASALIDAI = parametrizacion.AlmacenSalidaIny,
                U_AAPROBI = parametrizacion.AlmacenAprobadosIny,
                U_ARECHRECII = parametrizacion.AlmacenRechReciIny,
                U_ARECHNORECII = parametrizacion.AlmacenRechNoReciIny,
                U_ARETENIDOSI = parametrizacion.AlmacenRetenidosIny,
                U_ASALIDAE = parametrizacion.AlmacenSalidaExt,
                U_AAPROBE = parametrizacion.AlmacenAprobadosExt,
                U_ARECHRECIE = parametrizacion.AlmacenRechReciExt,
                U_ARECHNORECIE = parametrizacion.AlmacenRechNoReciExt,
                U_ARETENIDOSE = parametrizacion.AlmacenRetenidosExt,
                EEP_PARSERIE_DETCollection = listSeriesDet
            };

            _connectionService.SetEntitySL(method, entity, body, true);
        }
    }
}

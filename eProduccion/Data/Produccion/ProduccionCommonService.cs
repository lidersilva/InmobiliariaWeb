using eProduccion.Data.Configuracion;
using eProduccion.Models;
using RestSharp;

namespace eProduccion.Data.Produccion
{
    public class ProduccionCommonService(ConnectionService connectionService, ParametrizacionService parametrizacion)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly ParametrizacionService _parametrizacion = parametrizacion;

        public async Task<string> ObtenerCodigoEtapaRuta(string estacion)
        {
            var parametrizacion = await _parametrizacion.ObtenerParametrizacion();

            return estacion switch
            {
                "INYECCION" => parametrizacion.CodEstacionInyeccion,
                "EXTRUSION" => parametrizacion.CodEstacionExtrusion,
                "ARMADO" => parametrizacion.CodEstacionArmado,
                "FLOWPACK" => parametrizacion.CodEstacionFlowpack,
                "SACHETERA" => parametrizacion.CodEstacionSachetera,
                "SELLADO" => parametrizacion.CodEstacionSellado,
                "HORNEADO" => parametrizacion.CodEstacionHorneado,
                "EMPAQUETADO" => parametrizacion.CodEstacionEmpaquetado,
                "PRENSA" => parametrizacion.CodEstacionPrensa,
                "GRABADO LASER" => parametrizacion.CodEstacionGrabadoL,
            };
        }

        public string ObtenerTurnoSegunHoraSistema()
        {
            var horaActual = DateTime.Now.TimeOfDay;

            if (horaActual >= TimeSpan.FromHours(6) && horaActual < TimeSpan.FromHours(14))
                return "TM";
            if (horaActual >= TimeSpan.FromHours(14) && horaActual < TimeSpan.FromHours(22))
                return "TT";
            else
                return "TN";
        }
    }
}

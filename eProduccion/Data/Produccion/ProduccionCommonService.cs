using eProduccion.Data.Configuracion;

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
    }
}

using eProduccion.Data.Configuracion;
using eProduccion.Models;
using eProduccion.Utility;
using RestSharp;
using System.Data;
using System.Data.Odbc;
using System.Text;

namespace eProduccion.Data.Produccion
{
    public class MolinadoService(ConnectionService connectionService, ParametrizacionService parametrizacionService)
    {
        private readonly ConnectionService _connectionService = connectionService;
        private readonly ParametrizacionService _parametrizacionService = parametrizacionService;

    }
}

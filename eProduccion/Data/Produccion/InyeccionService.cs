using System.Data.Odbc;
using eProduccion.Models;

namespace eProduccion.Data.Produccion
{
    public class InyeccionService(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;


    }
}

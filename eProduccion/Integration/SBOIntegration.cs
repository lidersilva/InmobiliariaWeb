using eProduccion.Data;
using eProduccion.Models;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace eProduccion.Integration
{
    public class SBOIntegration(ConnectionService connectionService)
    {
        private readonly ConnectionService _connectionService = connectionService;

        #region InventoryGenExits
        public JObject CrearSalidaMercancias(int otReferencial, int lineIdReferencial, List<EntradaSalidaDet> salidaDet)
        {
            var method = Method.Post;
            var entity = $"InventoryGenExits";

            var listSalidaDetalle = new List<dynamic>();
            foreach (var i in salidaDet)
            {
                var bodyDet = new
                {
                    ItemCode = i.Articulo,
                    Quantity = i.Quantity,
                    BaseType = -1,
                    WarehouseCode = "PETROSAN",
                    AcctCode = "1.01.04.03.18",
                };

                listSalidaDetalle.Add(bodyDet);
            }

            var body = new
            {
                DocDate = DateTime.Now,
                Comments = $"Salida eProduccion\r\nInyeccion OT {otReferencial} Linea {lineIdReferencial}",
                U_OTREFE = otReferencial,
                U_LINEREFE = lineIdReferencial,
                DocumentLines = listSalidaDetalle,
            };

            var jObject = _connectionService.SetEntitySL(method, entity, body);

            return jObject;
        }
        #endregion
    }
}

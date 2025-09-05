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
        public JObject CrearSalidaMercancias(int otReferencial, int lineIdReferencial, List<EntradaSalidaDet> salidaDet, string cuentaSalida, string almacen, string estacion)
        {
            var method = Method.Post;
            var entity = $"InventoryGenExits";

            var listSalidaDetalle = new List<dynamic>();
            foreach (var i in salidaDet)
            {
                var bodyDet = new
                {
                    ItemCode = i.Articulo,
                    Quantity = i.Cantidad,
                    BaseType = -1,
                    WarehouseCode = almacen,
                    AcctCode = cuentaSalida,
                    BatchNumbers = i.LoteDetalle,
                };

                listSalidaDetalle.Add(bodyDet);
            }

            var body = new
            {
                DocDate = DateTime.Now,
                Comments = $"Salida eProducción\r\n{estacion} OT {otReferencial} Línea {lineIdReferencial}",
                U_OTREFE = otReferencial,
                U_LINEREFE = lineIdReferencial,
                DocumentLines = listSalidaDetalle,
            };

            var jObject = _connectionService.SetEntitySL(method, entity, body);

            return jObject;
        }
        #endregion

        #region JournalEntries
        public JObject CrearAsiento(int otReferencial, int lineIdReferencial, List<AsientoDet> asientoDet, string estacion, int docNumSalida)
        {
            var method = Method.Post;
            var entity = $"JournalEntries";

            var listAsientoDetalle = new List<dynamic>();
            foreach (var i in asientoDet)
            {
                var bodyDet = new
                {
                    AccountCode = i.AccountCode,
                    Debit = i.Debito,
                    Credit = i.Credito,
                };

                listAsientoDetalle.Add(bodyDet);
            }

            var body = new
            {
                ReferenceDate = DateTime.Now,
                TaxDate = DateTime.Now,
                Memo = $"Asiento eProducción {estacion} OT {otReferencial} Línea {lineIdReferencial} DocNum Salida {docNumSalida}",
                Reference = docNumSalida,
                Reference2 = otReferencial,
                Reference3 = lineIdReferencial,
                JournalEntryLines = listAsientoDetalle,
            };

            var jObject = _connectionService.SetEntitySL(method, entity, body);

            return jObject;
        }
        #endregion
    }
}

using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace eProduccion.Models
{
    public class Lote
    {
        [JsonProperty("BatchNumber")]
        public string NroLote { get; set; }
        [JsonProperty("Quantity")]
        public double Cantidad { get; set; }
    }
}

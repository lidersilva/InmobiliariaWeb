using System.Text.Json.Serialization;

namespace eProduccion.Models
{
    public class BaseDatos
    {
        [JsonPropertyName("nombre")]
        public string NombreBaseDatos {  get; set; } = string.Empty;
    }
}

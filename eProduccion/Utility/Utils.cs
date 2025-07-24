using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace eProduccion.Utility
{
    public class Utils
    {
        public static string JsonSerializeObject(object _object)
        {
            return JsonConvert.SerializeObject(_object
                , Formatting.None
                , new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static T JsonDeserializeObject<T>(string _object)
        {
            return JsonConvert.DeserializeObject<T>(_object
                , new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static void ReleaseObject(object oObject)
        {
            if (oObject != null)
            {
                Marshal.FinalReleaseComObject(oObject);
            }
        }
    }
}

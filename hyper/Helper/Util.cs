using Newtonsoft.Json;

namespace hyper.Helper
{
    public class Util
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings { Converters = { new ByteArrayHexConverter() } };

        public static string ObjToJson(object obj, bool format = true)
        {
            return JsonConvert.SerializeObject(obj, format ? Formatting.Indented : Formatting.None, settings).ToString();
        }
    }
}
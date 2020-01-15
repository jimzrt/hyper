using Newtonsoft.Json;
using System;

namespace hyper.Helper
{
    public class Util
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings { Converters = { new ByteArrayHexConverter() } };

        public static string ObjToJson(object obj, bool format = true)
        {
            return JsonConvert.SerializeObject(obj, format ? Formatting.Indented : Formatting.None, settings).ToString();
        }

        public static object JsonToObj(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
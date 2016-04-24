using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Rabbitual.Infrastructure
{
    public static class ObjectExtentions
    {
        public static string ToJson(this object o)
        {
            return JsonConvert.SerializeObject(o, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Rabbitual.Infrastructure
{
    public static class ObjectExtentions
    {
        public static string ToJson(this object o, bool bigAssPropertyNames=false)
        {
            var s = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = bigAssPropertyNames ? new DefaultContractResolver() : new CamelCasePropertyNamesContractResolver() 
            };

            s.Converters.Add(new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm'Z'"
            });

            return JsonConvert.SerializeObject(o, s);
        }
    }
}
using System;
using System.Text;
using Newtonsoft.Json;

namespace Rabbitual
{
    public class JsonSerializer : ISerializer
    {
        public byte[] ToBytes<T>(T o)
        {
            throw new NotImplementedException();
        }

        public T FromBytes<T>(byte[] body)
        {
            var message = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
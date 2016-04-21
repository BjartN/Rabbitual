using System.Collections.Generic;
using System.Text;

namespace Rabbitual
{
    public class Message
    {
        public Message()
        {
            Data = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Data { get; set; }
        public string SourceAgentId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in Data)
            {
                sb.Append(kvp.Key + "=" + kvp.Value + "&");
            }
            return sb.ToString().TrimEnd('&');
        }
    }
}

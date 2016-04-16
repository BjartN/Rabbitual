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

        public Message(string identifier, string type) : this(identifier, type, new Dictionary<string, string>())
        { }

        public Message(string identifier, string type, IDictionary<string, string> data)
        {
            Type = type;
            Data = data ?? new Dictionary<string, string>();
            Identifier = identifier;
        }

        public IDictionary<string, string> Data { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }

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

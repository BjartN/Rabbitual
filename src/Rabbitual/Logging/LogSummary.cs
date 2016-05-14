using Rabbitual.Core;

namespace Rabbitual.Logging
{
    public class LogSummary
    {
        public Message LastTaskIn { get; set; }
        public Message LastEventIn { get; set; }
        public Message LastTaskOut { get; set; }
        public Message LastEventOut { get; set; }
        public Message LastCheck { get; set; }
        public int OutgoingCount { get; set; }
        public int IncomingCount { get; set; }
    }
}
using System.Collections.Generic;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual
{
    public class AgentMessageLog : IAgentMessageLog
    {
        readonly List<Message> _outgoing = new List<Message>();
        readonly List<Message> _incoming = new List<Message>();
        private readonly ListManager<Message> _o;
        private readonly ListManager<Message> _i;
        private readonly LogSummary _logSummary;


        public AgentMessageLog()
        {
            _logSummary = new LogSummary();
            _o = new ListManager<Message>(_outgoing, 100);
            _i = new ListManager<Message>(_incoming, 100);
        }

        public Message[] GetIncoming()
        {
            return _incoming.ToArray();
        }

        public Message[] GetOutGoing()
        {
            return _outgoing.ToArray();
        }

        public LogSummary GetSummary()
        {
            return _logSummary;
        }


        public void LogIncoming(Message m)
        {
            switch (m.MessageType)
            {
                case MessageType.Check:
                    _logSummary.LastCheck = m;
                    break;
                case MessageType.Event:
                    _logSummary.LastEventIn = m;
                    break;
                case MessageType.Task:
                    _logSummary.LastTaskIn = m;
                    break;
            }

            _i.Add(m);

            if(m.MessageType!=MessageType.Check)
                _logSummary.IncomingCount++;
        }

        public void LogOutgoing(Message m)
        {
            switch (m.MessageType)
            {
                case MessageType.Event:
                    _logSummary.LastEventOut = m;
                    break;
                case MessageType.Task:
                    _logSummary.LastTaskOut = m;
                    break;
            }

            _o.Add(m);

            if (m.MessageType != MessageType.Check)
                _logSummary.OutgoingCount++;
        }
    }
}
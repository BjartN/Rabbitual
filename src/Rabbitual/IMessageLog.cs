using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public interface IAgentMessageLog
    {
        void LogIncoming(Message m);
        void LogOutgoing(Message m);

        Message[] GetIncoming();
        Message[] GetOutGoing();

        Summary GetSummary();
    }

    public interface IAgentLogRepository
    {
        IAgentMessageLog GetLog(string agentId);
    }

    public class Summary
    {
        public Message LastTaskIn { get; set; }
        public Message LastEventIn { get; set; }
        public Message LastTaskOut { get; set; }
        public Message LastEventOut { get; set; }
        public Message LastCheck { get; set; }
        public int OutgoingCount { get; set; }
        public int IncomingCount { get; set; }
    }

    public class AgentMessageLog : IAgentMessageLog
    {
        readonly List<Message> _outgoing = new List<Message>();
        readonly List<Message> _incoming = new List<Message>();
        private readonly ListManager<Message> _o;
        private readonly ListManager<Message> _i;
        private readonly Summary _summary;


        public AgentMessageLog()
        {
            _summary = new Summary();
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

        public Summary GetSummary()
        {
            return _summary;
        }


        public void LogIncoming(Message m)
        {
            switch (m.MessageType)
            {
                case MessageType.Check:
                    _summary.LastCheck = m;
                    break;
                case MessageType.Event:
                    _summary.LastEventIn = m;
                    break;
                case MessageType.Task:
                    _summary.LastTaskIn = m;
                    break;
            }

            _i.Add(m);
            _summary.IncomingCount++;
        }

        public void LogOutgoing(Message m)
        {
            switch (m.MessageType)
            {
                case MessageType.Event:
                    _summary.LastEventOut = m;
                    break;
                case MessageType.Task:
                    _summary.LastTaskOut = m;
                    break;
            }

            _o.Add(m);
            _summary.OutgoingCount++;
        }
    }

    public class AgentLogRepository : IAgentLogRepository
    {
        readonly IDictionary<string, AgentMessageLog> _logs = new ConcurrentDictionary<string, AgentMessageLog>();

        public IAgentMessageLog GetLog(string agentId)
        {
            if (!_logs.ContainsKey(agentId))
                _logs[agentId] = new AgentMessageLog();

            return _logs[agentId];
        }
    }
}
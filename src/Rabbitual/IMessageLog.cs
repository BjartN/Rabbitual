using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public interface IAgentMessageLog
    {
        void LogIncoming( Message m);
        void LogOutgoing( Message m);

        Message[] GetIncoming();
        Message[] GetOutGoing();
    }

    public interface IAgentLogRepository
    {
        IAgentMessageLog GetLog(string agentId);
    }

    public class AgentMessageLog: IAgentMessageLog
    {
        readonly List<Message> _outgoing = new List<Message>();
        readonly List<Message> _incoming = new List<Message>();
        private readonly ListManager<Message> _o;
        private readonly ListManager<Message> _i;


        public AgentMessageLog()
        {
            _o = new ListManager<Message>(_outgoing,100);
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

        public void LogIncoming(Message m)
        {
           _i.Add(m);
        }

        public void LogOutgoing(Message m)
        {
            _o.Add(m);
        }
    }

    public class AgentLogRepository: IAgentLogRepository
    {
        readonly IDictionary<string,AgentMessageLog> _logs = new ConcurrentDictionary<string, AgentMessageLog>();

        public IAgentMessageLog GetLog(string agentId)
        {
            if(!_logs.ContainsKey(agentId))
                _logs[agentId]= new AgentMessageLog();

            return _logs[agentId];
        }
    }
}
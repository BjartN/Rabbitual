using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rabbitual.Logging
{
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
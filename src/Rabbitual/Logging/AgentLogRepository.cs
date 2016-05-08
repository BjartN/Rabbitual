using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rabbitual.Logging
{
    public class AgentLogRepository : IAgentLogRepository
    {
        readonly IDictionary<int, AgentMessageLog> _logs = new ConcurrentDictionary<int, AgentMessageLog>();

        public IAgentMessageLog GetLog(int agentId)
        {
            if (!_logs.ContainsKey(agentId))
                _logs[agentId] = new AgentMessageLog();

            return _logs[agentId];
        }
    }
}
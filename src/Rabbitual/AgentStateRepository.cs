using Rabbitual.Core;
using Rabbitual.Core.Logging;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual
{
    public class AgentStateRepository : IAgentStateRepository
    {
        private readonly int _agentId;
        private readonly IAgentDb _db;
        private readonly ILogger _log;

        public AgentStateRepository(int agentId, IAgentDb db, ILogger log)
        {
            _agentId = agentId;
            _db = db;
            _log = log;
        }

        public T GetState<T>()
        {
            return _db.GetState<T>(_agentId);
        }

        public void PersistState(object state)
        {
            _log.Info("Persisting state for {0}", _log);
            _db.InsertOrReplaceState(_agentId,state);
        }
    }
}
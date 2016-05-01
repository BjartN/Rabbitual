using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class AgentStateRepository : IAgentStateRepository
    {
        private readonly string _agentId;
        private readonly IObjectDb _db;
        private readonly ILogger _log;

        public AgentStateRepository(string agentId, IObjectDb db, ILogger log)
        {
            _agentId = agentId;
            _db = db;
            _log = log;
        }

        public T GetState<T>()
        {
            return _db.Get<T>("state." + _agentId);
        }

        public void PersistState(object state)
        {
            _log.Info("Persisting state for {0}", _log);
            _db.Save(state,"state." + _agentId);
        }
    }
}
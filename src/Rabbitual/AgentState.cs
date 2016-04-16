using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class AgentState : IAgentState
    {
        private readonly string _agentId;
        private readonly IObjectDb _db;

        public AgentState(string agentId, IObjectDb db)
        {
            _agentId = agentId;
            _db = db;
        }

        public object GetState()
        {
            return _db.Get("state." + _agentId);
        }

        public void PersistState(object state)
        {
            _db.Save(state,"state." + _agentId);
        }
    }
}
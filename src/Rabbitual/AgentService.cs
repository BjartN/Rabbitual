using System;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public interface IAgentService
    {
        object GetState(IAgent a);
        object GetPersistedState(Type agent, string id);
    }


    public class AgentService : IAgentService
    {
        private readonly IObjectDb _db;
        private readonly ILogger _logger;

        public AgentService(
            IObjectDb db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public object GetPersistedState(Type agent, string id)
        {
            if (!agent.IsOfType(typeof(IStatefulAgent<>)))
                return null;

            var service = new AgentStateRepository(id, _db, _logger);
            return StateHelper.GetPersistedStateUsingMagic(service, agent);
        }

        public object GetState(IAgent a)
        {
            if (a == null || !(a.GetType().IsOfType(typeof(IStatefulAgent<>))))
                return null;

            return StateHelper.GetStateUsingMagic(a);
        }
    }
}
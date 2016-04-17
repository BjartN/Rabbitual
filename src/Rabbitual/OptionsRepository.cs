using System;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class OptionsRepository: IOptionsRepository
    {
        private readonly IObjectDb _db;

        public OptionsRepository(IObjectDb db)
        {
            _db = db;
        }

        public object GetOptions(Type agent, string agentId)
        {
            var result = _db.Get(agent, "option." + agentId);
            if (result != null)
                return result;
            var optionType  =ReflectionHelper.GetOptionType(agent);
            return Activator.CreateInstance(optionType);
        }
    }
}
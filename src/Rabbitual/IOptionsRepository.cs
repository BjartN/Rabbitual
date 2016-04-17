using System;

namespace Rabbitual
{
    public interface IOptionsRepository
    {
        object GetOptions(Type agent, string agentId);
    }
}
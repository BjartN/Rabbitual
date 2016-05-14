namespace Rabbitual.Core
{
    public interface IAgentStateRepository
    {
        T GetState<T>();
        void PersistState(object state);
    }
}
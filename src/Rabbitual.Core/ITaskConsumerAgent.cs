namespace Rabbitual.Core
{
    /// <summary>
    ///     Execute work 
    /// </summary>
    public interface ITaskConsumerAgent : IAgent
    {
        bool CanDoWork(Message task);
        void DoWork(Message task);
    }
}
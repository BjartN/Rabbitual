using Rabbitual.Configuration;
using Rabbitual.Core;

namespace Rabbitual
{
    public interface IAgentWrapper
    {
        AgentConfig Config { get; set; }
        int Id { get; set; }
        void Start();
        void Stop();

        //Statefullness
        bool HasState();
        object GetState();

        //Scheduling
        void Check();
        bool IsScheduled();
        int GetSchedule();

        //Pub Sub
        bool IsPublisher();
        bool IsConsumer();
        bool CanConsume(int fromAgentId);
        void Consume(Message message);


        //Producer consumer
        bool IsWorker();
        bool CanDoWork(Message message);
        void DoWork(Message message);
    }
}
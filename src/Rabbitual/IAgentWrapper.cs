using System.Threading.Tasks.Dataflow;
using Rabbitual.Configuration;

namespace Rabbitual
{
    public interface IAgentWrapper
    {
        AgentConfig Config { get; set; }
        string Id { get; set; }
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
        bool CanConsume(string fromAgentId);
        void Consume(Message message);


        //Producer consumer
        bool IsWorker();
        bool CanDoWork(Message message);
        void DoWork(Message message);
    }
}
using System.Threading.Tasks.Dataflow;

namespace Rabbitual
{
    public interface IAgentWrapper
    {
        BufferBlock<Message> Buffer { get; }

        string Id { get; set; }

        //Statefullness
        bool HasState();
        object GetState();

        void Start();
        void Stop();
        void Check();

        //Scheduling
        bool IsScheduled();
        int GetSchedule();

        //Pub Sub
        bool IsPublisher();
        bool IsConsumer();
        void Consume(Message message);
        bool CanConsume(string fromAgentId);


        //Producer consumer
        bool IsWorker();
        bool CanDoWork(Message message);
        void DoWork(Message message);
    }
}
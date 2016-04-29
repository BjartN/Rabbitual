namespace Rabbitual
{
    public interface IAgentWrapper
    {
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


        //Producer consumer
        bool IsWorker();
        bool CanDoWork(Message message);
        void DoWork(Message message);
    }
}
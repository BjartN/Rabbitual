namespace Rabbitual.Fox
{
    public class FoxTaskConsumer : ITaskConsumer
    {
        private readonly Hub _hub;

        public FoxTaskConsumer(Hub hub)
        {
            _hub = hub;
        }

        public void Start(IAgentWrapper w)
        {
            _hub.AddWorker(w);
        }

        public void Stop()
        {

        }
    }
}
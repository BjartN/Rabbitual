namespace Rabbitual.Fox
{
    public class FoxEventConsumer : IEventConsumer
    {
        private readonly Hub _hub;

        public FoxEventConsumer(Hub hub)
        {
            _hub = hub;
        }

        public void Start(IAgentWrapper a)
        {
            _hub.Subscribe(a);
        }
    }
}
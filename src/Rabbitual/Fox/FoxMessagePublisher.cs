namespace Rabbitual.Fox
{
    public class FoxMessagePublisher: IPublisher
    {
        private readonly MessageHub _hub;

        public FoxMessagePublisher(MessageHub hub)
        {
            _hub = hub;
        }

        public void SubmitTask(Message m)
        {
            _hub.Publish(m);
        }

        public void PublishEvent(Message m)
        {
            _hub.Publish(m);
        }
    }
}
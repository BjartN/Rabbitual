namespace Rabbitual.Fox
{
    public class FoxMessagePublisher: IPublisher
    {
        private readonly EventHub _hub;
        private readonly TaskHub _taskHub;

        public FoxMessagePublisher(EventHub hub,TaskHub taskHub)
        {
            _hub = hub;
            _taskHub = taskHub;
        }

        public void EnqueueTask(Message task)
        {
            _taskHub.EnqueueTask(task);
        }

        public void PublishEvent(Message e)
        {
            _hub.PublishEvent(e);
        }
    }
}
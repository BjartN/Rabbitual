using Rabbitual.Configuration;

namespace Rabbitual
{
    public class PublisherProxy : IPublisher
    {
        private readonly IPublisher _inner;
        private readonly string _agentId;

        public PublisherProxy(IPublisher inner, string agentId)
        {
            _inner = inner;
            _agentId = agentId;
        }

        public void EnqueueTask(Message task)
        {
            task.SourceAgentId = _agentId;
            _inner.EnqueueTask(task);
        }

        public void PublishEvent(Message e)
        {
            e.SourceAgentId = _agentId;
            _inner.PublishEvent(e);
        }
    }
}
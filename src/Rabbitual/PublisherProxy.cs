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

        public void SubmitTask(Message m)
        {
            m.SourceAgentId = _agentId;
            _inner.SubmitTask(m);
        }

        public void PublishEvent(Message m)
        {
            m.SourceAgentId = _agentId;
            _inner.PublishEvent(m);
        }
    }
}
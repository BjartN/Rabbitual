
namespace Rabbitual
{
    public class PublisherProxy : IPublisher
    {
        private readonly IPublisher _inner;
        private readonly string _agentId;
        private readonly IAgentMessageLog _messageLog;

        public PublisherProxy(IPublisher inner, string agentId, IAgentLogRepository log)
        {
            _inner = inner;
            _agentId = agentId;
            _messageLog = log.GetLog(agentId);
        }

        public void EnqueueTask(Message task)
        {
            _messageLog.LogOutgoing(task);

            task.SourceAgentId = _agentId;
            _inner.EnqueueTask(task);
        }

        public void PublishEvent(Message e)
        {
            _messageLog.LogOutgoing(e);

            e.SourceAgentId = _agentId;
            _inner.PublishEvent(e);
        }
    }
}
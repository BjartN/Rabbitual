
namespace Rabbitual
{
    public class PublisherProxy : IMessagePublisher
    {
        private readonly IMessagePublisher _inner;
        private readonly string _agentId;
        private readonly IAgentMessageLog _messageLog;

        public PublisherProxy(IMessagePublisher inner, string agentId, IAgentLogRepository log)
        {
            _inner = inner;
            _agentId = agentId;
            _messageLog = log.GetLog(agentId);
        }

        public void EnqueueTask(Message task)
        {
            task.SourceAgentId = _agentId;
            task.MessageType = MessageType.Task;
            _messageLog.LogOutgoing(task);

            _inner.EnqueueTask(task);
        }

        public void PublishEvent(Message e)
        {
            e.SourceAgentId = _agentId;
            e.MessageType = MessageType.Event;
            _messageLog.LogOutgoing(e);

            _inner.PublishEvent(e);
        }
    }
}
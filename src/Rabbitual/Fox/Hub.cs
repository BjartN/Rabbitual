using System.Threading.Tasks.Dataflow;

namespace Rabbitual.Fox
{

    public class Hub
    {
        //producer consumer
        private readonly BufferBlock<Message> _workBlock;
       
        //pub sub
        private readonly BroadcastBlock<Message> _broadcastBlock;

        public Hub()
        {
            _broadcastBlock = new BroadcastBlock<Message>(x => x);
            _workBlock = new BufferBlock<Message>();
        }

        public void PublishEvent(Message message)
        {
            message.MessageType = MessageType.Event;
            _broadcastBlock.Post(message);
        }

        public void EnqueueTask(Message message)
        {
            message.MessageType = MessageType.Event;
            _workBlock.Post(message);
        }

        public void Subscribe(IAgentWrapper w)
        {
            var ab = new ActionBlock<Message>(m=>w.Consume(m));
            _broadcastBlock.LinkTo(ab, m=> w.CanConsume(m.SourceAgentId));
        }

        public void AddWorker(IAgentWrapper w)
        {
            var ab = new ActionBlock<Message>(m => w.DoWork(m));
            _workBlock.LinkTo(ab,w.CanDoWork);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rabbitual.Infrastructure;

namespace Rabbitual.Fox
{

    public class Hub
    {
        private readonly ILogger _log;
        
        //producer consumer
        private readonly BufferBlock<Message> _workBlock;
        private int _workCount;

        //pub sub
        private readonly BroadcastBlock<Message> _broadcastBlock;

        //single access per agent
        private readonly ConcurrentDictionary<string,BufferBlock<Message>> _agentBuffers = new ConcurrentDictionary<string, BufferBlock<Message>>(); 


        public Hub(ILogger log)
        {
            _log = log;

            _broadcastBlock = new BroadcastBlock<Message>(clone);
            _workBlock = new BufferBlock<Message>();
        }

        public void PublishEvent(Message message)
        {
            _broadcastBlock.Post(message);
        }

        public void EnqueueTask(Message task)
        {
            _log.Debug("Adding work #" + (_workCount++));
            _workBlock.Post(task);
        }

        public void Subscribe(Action<Message> callback, string id)
        {
            var action = new ActionBlock<Message>(callback);
            var agentBuffer = _agentBuffers.GetOrAdd(id, s => new BufferBlock<Message>());

            agentBuffer.LinkTo(action);
            _broadcastBlock.LinkTo(agentBuffer);
        }

        public void AddWorker(ITaskConsumerAgent worker)
        {
            var work = new ActionBlock<Message>(m =>
            {
                Task.Run(() => worker.DoWork(m));
            });
            var agentBuffer = _agentBuffers.GetOrAdd(worker.Id, s => new BufferBlock<Message>());

            agentBuffer.LinkTo(work);
            _workBlock.LinkTo(agentBuffer, worker.CanDoWork);
            _log.Debug("Adding worker agent #" + worker.Id);
        }

        private Message clone(Message x)
        {
            return x;
        }

    }


}
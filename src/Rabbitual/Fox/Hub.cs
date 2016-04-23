using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rabbitual.Infrastructure;

namespace Rabbitual.Fox
{
    public class Hub
    {
        private readonly ILogger _log;
        
        //producer consumer
        private BufferBlock<Message> _work;
        private BufferBlock<Action<Message>> _workers;
        private JoinBlock<Message, Action<Message>> _jb;
        private int _workCount;
        private int _workerCount;

        //pub sub
        private BroadcastBlock<Message> _bb;

        public Hub(ILogger log)
        {
            _log = log;

            initPubSub();

            initProducerConsumer();
            Task.Run(() => consumeWork());
        }

        private void initPubSub()
        {
             _bb = new BroadcastBlock<Message>(clone);
        }

        private Message clone(Message x)
        {
            return x;
        }

        private void initProducerConsumer()
        {
            _work = new BufferBlock<Message>();
            _workers = new BufferBlock<Action<Message>>();
            _jb = new JoinBlock<Message, Action<Message>>(new GroupingDataflowBlockOptions {Greedy = true});
            _work.LinkTo(_jb.Target1);
            _workers.LinkTo(_jb.Target2);
        }

        public void PublishEvent(Message message)
        {
            _bb.Post(message);
        }

        public void Subscribe(Action<Message> callback)
        {
            var daAction = new ActionBlock<Message>(callback);
            _bb.LinkTo(daAction);
        }

        public void EnqueueTask(Message task)
        {
            _log.Debug("Adding work #" + (_workCount++));
            _work.Post(task);
        }

        public void AddWorker(Action<Message> callback)
        {
            _log.Debug("Adding worker #" + (_workerCount++));
            _workers.Post(callback);
        }

        private async void consumeWork()
        {
            while (true)
            {
                _log.Debug("Start waiting");
                var workAndWorker = await _jb.ReceiveAsync();
                _log.Debug("Found work for task");

                //fire and forget 
                Task.Run(() =>
                {
                    workAndWorker.Item2(workAndWorker.Item1);
                    AddWorker(workAndWorker.Item2);
                    _log.Debug("Work Done. Worker back on stack");
                });
            }
        }
    }
}
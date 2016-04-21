using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rabbitual.Infrastructure;

namespace Rabbitual.Fox
{
    public class TaskHub
    {
        private readonly ILogger _log;
        private readonly ConcurrentQueue<Message> _work = new ConcurrentQueue<Message>();
        private readonly ConcurrentQueue<Action<Message>> _workers = new ConcurrentQueue<Action<Message>>();
        private readonly object _lock = new object();

        public TaskHub(ILogger log)
        {
            _log = log;
        }

        bool _started = false;
        public void EnqueueTask(Message task)
        {
            _work.Enqueue(task);
            if (_started)
                return;
            _started = true;
            Task.Factory.StartNew(deliverWork);
        }

        public void AddWorker(Action<Message> callback)
        {
            lock (_lock)
            {
                _workers.Enqueue(callback);
            }
        }

        private int _inProgress;

        private void deliverWork()
        {
            while (true)
            {
                if (!_work.Any())
                    break;

                foreach (var worker in _workers)
                {
                    if (_inProgress > 20)
                    {
                        //We don't want to create to many tasks
                         break;
                    }

                    Message work;
                    _work.TryDequeue(out work);
                    if (work == null)
                        continue;

                    //Wonder if this will work as intended.
                    _inProgress++;
                    Task.Run(() =>
                    {
                        try
                        {
                            worker(work);
                            _inProgress--;
                        }
                        catch
                        {
                            _work.Enqueue(work);
                            _inProgress--;
                        }
                    });
                }

                //reorder queue
                Action<Message> wx;
                if (_workers.TryDequeue(out wx))
                {
                    _workers.Enqueue(wx);
                }
            }
        }
    }
}
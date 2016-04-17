using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rabbitual.Fox
{
    /// <summary>
    /// Simple naive implementation of a in memory message buss
    /// 
    /// Rabbit is the read deal, the Fox just makes life easier when developing.
    /// </summary>
    public class MessageHub
    {
        private readonly List<Action<object>> _subscribers = new List<Action<object>>();
        readonly object _locker = new object();

        public void Publish(object message)
        {
            lock (_locker)
            {
                foreach (var callback in _subscribers)
                    Task.Run(()=>callback(message));
            }
        }

        public void Subscribe(Action<object> callback)
        {
            lock(_locker)
                _subscribers.Add(callback);
        }
    }
}
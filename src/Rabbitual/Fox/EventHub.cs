using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rabbitual.Fox
{
    /// <summary>
    /// Simple naive implementation of a in memory task buss
    /// 
    /// Rabbit is the read deal, the Fox just makes life easier when developing.
    /// </summary>
    //public class EventHub
    //{
    //    private readonly List<Action<Message>> _subscribers = new List<Action<Message>>();
    //    private readonly object _locker = new object();

    //    public void PublishEvent(Message message)
    //    {
    //        lock (_locker)
    //        {
    //            foreach (var callback in _subscribers)
    //                Task.Run(() => callback(message));
    //        }
    //    }

    //    public void Subscribe(Action<Message> callback)
    //    {
    //        lock (_locker)
    //            _subscribers.Add(callback);
    //    }

    //}
}
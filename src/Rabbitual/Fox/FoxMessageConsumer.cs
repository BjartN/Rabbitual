namespace Rabbitual.Fox
{
    public class FoxMessageConsumer:IMessageConsumer
    {
        private readonly MessageHub _m;

        public FoxMessageConsumer(MessageHub m)
        {
            _m = m;
        }

        public void Start(IEventConsumerAgent[] agents)
        {
            foreach(var a in agents)
                _m.Subscribe(message=>a.Consume(message));
        }

        public void Stop()
        {
            
        }
    }
}
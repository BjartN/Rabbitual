namespace Rabbitual.Rabbit.Fake
{
    public class TimedFakeMessageConsumer: IMessageConsumer
    {
        private Timer _timer;

        public TimedFakeMessageConsumer()
        {
            _timer = new Timer();
        }

        public void Start(IConsumerAgent[] agents)
        {
            _timer.Start(100,() =>
            {
                foreach (var agent in agents)
                {
                    agent.Consume(new FakeMessage());
                }
            });
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public class FakeMessage
    {
        
    }
}
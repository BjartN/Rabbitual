namespace Rabbitual.Rabbit.Fake
{
    public class TimedFakeMessageConsumer: IMessageConsumer
    {
        public void Start(IConsumerAgent[] agents)
        {
            
            new Timer().DoOnTimer(1,() =>
            {
                foreach (var agent in agents)
                {
                    agent.Consume(new FakeMessage());
                }
            });

        }
    }

    public class FakeMessage
    {
        
    }
}
using Rabbitual.Rabbit;

namespace Rabbitual.Console
{
    public class TestAgent:IConsumerAgent
    {
        private readonly ILogger _log;

        public TestAgent(ILogger log)
        {
            _log = log;
        }

        public bool CanConsume(object evt)
        {
            return true;
        }

        public void Consume(object evt)
        {
            _log.Log("I'm all in");
        }
    }
}
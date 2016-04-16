using RabbitMQ.Client;

namespace Rabbitual.Rabbit
{
    public class ExchangeDefinitions
    {
        public static void ExchangeDeclare(string exchangeName, IModel channel)
        {
            channel.ExchangeDeclare(exchangeName, "fanout");
        }
    }
}
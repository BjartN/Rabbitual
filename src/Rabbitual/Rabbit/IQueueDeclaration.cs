using RabbitMQ.Client;

namespace Rabbitual.Rabbit
{
    public interface IQueueDeclaration
    {
        void DeclareQueue(string queueName, IModel definition);
    }
}
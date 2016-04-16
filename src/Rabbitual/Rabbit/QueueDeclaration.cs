using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Rabbitual.Rabbit
{
    public class QueueDeclaration: IQueueDeclaration
    {
        public void DeclareQueue(string queueName, IModel channel)
        {
            if (queueName == Constants.TaskQueue)
            {
                DeclareQueue(channel, queueName, 1000 * 60 * 60 * 12);
                return;
            }

            throw new NotImplementedException();
        }

        public static void DeclareQueue(IModel channel, string queueName, int ttl)
        {
            channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                //remove message from queue after half a day
                arguments: new Dictionary<string, object> { { "x-message-ttl", ttl } }
                );
        }

    }
}
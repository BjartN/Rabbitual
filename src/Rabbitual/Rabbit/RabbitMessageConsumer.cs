using System;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Rabbitual.Rabbit
{
    /// <summary>
    /// Recieve from RabbitMq and delegate work to typed consumersAgent
    /// </summary>
    public class RabbitMessageConsumer : IMessageConsumer
    {
        private readonly ILogger _c;
        private readonly ISerializer _s;
        private readonly IQueueDeclaration _declarations;
        private readonly string _queueName;

        private readonly ConnectionFactory _factory;
        private IModel _channel;
        private IConnection _connection;

        public RabbitMessageConsumer(
            ILogger c,
            IConfiguration cfg,
            ISerializer s,
            IQueueDeclaration declarations,
            string queueName)
        {
            _c = c;
            _s = s;
            _declarations = declarations;
            _queueName = queueName;
            _factory = new ConnectionFactory
            {
                HostName = cfg.Get("rabbit.hostname")
            };
        }


        public void Start(IConsumerAgent[] agents)
        {
            try
            {
                _connection = _factory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                _c.Log("Could not connect to Rabbit using HostName=" + _factory.HostName);
                _c.Log(ex.Message);
                return;
            }
            _channel = _connection.CreateModel();
            _declarations.DeclareQueue(_queueName, _channel);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            //recieve tasks and send to all that can process it
            StartRecieve<Message>(t =>
            {
                foreach (var taskProcessor in agents.Where(x => x.CanConsume(t)))
                    taskProcessor.Consume(t);
            });
        }

        public void Stop()
        {
            _channel.Close();
            _channel.Dispose();

            _connection.Close();
            _connection.Dispose();
        }

        public void StartRecieve<T>(Action<T> doWork)
        {
            var consumer = new EventingBasicConsumer(_channel);

            _c.Log("Waiting for tasks.");
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var task = _s.FromBytes<T>(body);

                _c.Log("Message received {0} ", task.GetType());

                try
                {
                    doWork(task);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    _c.Log("Message processed at {0}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _channel.BasicReject(ea.DeliveryTag, true);
                    _c.Log("Error");
                    _c.Log(ex.Message);
                    _c.Log(ex.StackTrace);
                }
            };

            _channel.BasicConsume(queue: Constants.TaskQueue, noAck: false, consumer: consumer);
        }

    }
}
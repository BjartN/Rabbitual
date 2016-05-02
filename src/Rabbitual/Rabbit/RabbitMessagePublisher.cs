using RabbitMQ.Client;
using Rabbitual.Infrastructure;

namespace Rabbitual.Rabbit
{
    /// <summary>
    ///  Publish / Enqueue messages to RabbitMq
    /// </summary>
    public class RabbitMessagePublisher : IMessagePublisher
    {
        private readonly IBinarySerializer _s;
        private readonly IAppConfiguration _cfg;
        private readonly IQueueDeclaration _declaration;

        public RabbitMessagePublisher(IBinarySerializer s, IAppConfiguration cfg, IQueueDeclaration declaration)
        {
            _s = s;
            _cfg = cfg;
            _declaration = declaration;
        }

        /// <summary>
        /// https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html
        /// 
        /// Following command in RabbitMq Command Prompt to enable admin on http://localhost:55672/mgmt/ 
        /// with username guest / guest
        /// 
        /// rabbitmq-plugins.bat enable rabbitmq_management 
        /// rabbitmq-service.bat stop 
        /// rabbitmq-service.bat install 
        /// rabbitmq-service.bat start 
        /// </summary>
        public void EnqueueTask(Message task)
        {
            SubmitTask(task, Constants.TaskQueue);
        }

        public void SubmitTask(Message m, string queueName)
        {
            var factory = new ConnectionFactory { HostName = _cfg.Get("rabbit.hostname"), UserName= _cfg.Get("rabbit.username"), Password = _cfg.Get("rabbit.password") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    _declaration.DeclareQueue(queueName, channel);

                    var body = _s.ToBytes(m);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                        routingKey: queueName,
                        basicProperties: properties,
                        body: body);

                    sinkIt(channel,body);
                }
            }
        }

        public void PublishEvent(Message e)
        {
            PublishEvent(e,Constants.PubSubExchange);
        }

        public void PublishEvent(Message e,string exchangeName)
        {
            var factory = new ConnectionFactory { HostName = _cfg.Get("rabbit.hostname") };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var body = _s.ToBytes(e);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    ExchangeDefinitions.ExchangeDeclare(exchangeName, channel);
                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body);

                    sinkIt(channel, body);
                }
            }
        }

        /// <summary>
        /// Publish everything to the sink if so desired by the setting "rabbitual.sink" set to "true".
        /// </summary>
        private void sinkIt(IModel channel, byte[] body)
        {
            if (_cfg.Get("rabbitual.sink") == "true")
            {
                //also pusblish everything in a PubSub fashion to the kitchen sink
                ExchangeDefinitions.ExchangeDeclare(Constants.KitchenSink, channel);
                channel.BasicPublish(exchange: Constants.KitchenSink,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
            }
        }
    }
}
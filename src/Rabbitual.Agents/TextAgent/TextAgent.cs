using System.Collections.Generic;
using Rabbitual.Core;
using Rabbitual.Core.Infrastructure;

namespace Rabbitual.Agents.TextAgent
{
    [Icon("comment")]
    public class TextAgent: Agent<TextOptions>, 
        IEventConsumerAgent, 
        IEventPublisherAgent
    {
        private readonly IMessagePublisher _publisher;

        public TextAgent(TextOptions options, IMessagePublisher publisher) : base(options)
        {
            _publisher = publisher;
        }

        public void Consume(Message evt)
        {
            var text = Options.Template.FromTemplate(evt.Data);
           
            _publisher.PublishEvent(new Message
            {
                Data= new Dictionary<string, string> { {"text", text} }
            });
        }
    }

    public class TextOptions
    {
        public TextOptions()
        {
            Template = "Hello world";
        }

        public string Template { get; set; }
    }
    
}
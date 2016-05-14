using System.Net;
using Rabbitual.Core;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.CsvAgent
{
    [Icon("file-text")]
    public class CsvAgent : ScheduledAgent<CsvOptions>, 
        IEventPublisherAgent
    {
        private readonly IMessagePublisher _publisher;
        private readonly ILogger _log;

        public CsvAgent(CsvOptions options, IMessagePublisher publisher, ILogger log) : base(options)
        {
            _publisher = publisher;
            _log = log;
        }

        public override void Check()
        {
            var url = Options.Url;

            string content;
            try
            {
                content = new WebClient().DownloadString(url);
            }
            catch
            {
                _log.Warn($"Failed fetching {url}");
                return;
            }

            var lines = CsvParser.Parse(content, Options);

            foreach (var line in lines)
            {
                _publisher.PublishEvent(new Message { Data = line });
            }
        }
    }
}

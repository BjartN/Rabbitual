using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using Rabbitual.Core;
using Rabbitual.Core.Infrastructure;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.EmailAgent
{
    [Icon("send")]
    public class EmailAgent : StatefulAgent<EmailOptions, EmailState>, IEventConsumerAgent
    {
        private readonly ILogger _logger;

        public EmailAgent(EmailOptions options, ILogger logger, IAgentStateRepository asr) : base(options, asr)
        {
            _logger = logger;
        }

        public void Consume(Message evt)
        {
            var now = DateTime.UtcNow.ToString("yyyyMMddHH");
            if (!State.EmailCount.ContainsKey(now))
                State.EmailCount[now] = 0;

            if (Options.MaxEmailCountPerHour.HasValue && State.EmailCount[now] > Options.MaxEmailCountPerHour.Value)
            {
                _logger.Warn("Email not sent due to limit on MaxEmailCountPerHour");
                return;
            }

            var message = new MailMessage();
            message.From = new MailAddress(Options.FromEmail);
            message.Subject = Options.SubjectTemplate.FromTemplate(evt.Data);
            message.Body = Options.BodyTemplate.FromTemplate(evt.Data);

            foreach (var address in Options.ToEmail.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(address);
            }

            try
            {
                var client = new SmtpClient
                {
                    Host = Options.Host,
                    Port = Options.Port,
                    EnableSsl = Options.EnableSsl,
                    Credentials = new NetworkCredential(Options.UserName, Options.Password)
                };

                client.Send(message);
                State.EmailCount[now] = State.EmailCount[now] + 1;
            }
            catch (Exception ex)
            {
                _logger.Warn("Could not send email");
                _logger.Warn(ex.Message);
            }
        }
    }

    public class EmailState
    {
        public EmailState()
        {
            EmailCount = new Dictionary<string, int>();
        }

        public IDictionary<string, int> EmailCount { get; set; }
    }

    public class EmailOptions
    {
        public EmailOptions()
        {
            MaxEmailCountPerHour = 10; //ensure sanity
        }

        [Description("Prevent hammering e-mail server")]
        public int? MaxEmailCountPerHour { get; set; }
        public string SubjectTemplate { get; set; }
        [Description("Semi-colon separated list of addressess")]
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string BodyTemplate { get; set; }
        public string Host { get; set; }
        public bool EnableSsl { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
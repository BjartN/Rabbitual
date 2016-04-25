using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebCheckerAgent
{
    public class WebCheckerAgent : ScheduledAgent<WebCheckerOptions>,
        IEventPublisherAgent
    {
        private readonly ILogger _l;
        private readonly IPublisher _p;

        public WebCheckerAgent(ILogger l,WebCheckerOptions options, IPublisher p) : base(options)
        {
            _l = l;
            _p = p;
        }

        public new int DefaultSchedule => 4000;

        /// <summary>
        /// Use xpath to match node, and check for text in node using RegEx.
        /// Output the inner text of every matched node as an event.
        /// </summary>
        public override void Check()
        {
            string contents;
            using (var wc = new System.Net.WebClient())
            {
                _l.Debug("Checking {0}", Options.Url);
                contents = wc.DownloadString(Options.Url);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(contents);

            var matchedNodes = doc.DocumentNode.SelectNodes(Options.XPath);


            var r = Options.Regex == null ? null : new Regex(Options.Regex);
            foreach (var n in matchedNodes)
            {
                if (r != null)
                {
                    if (r.IsMatch(n.InnerText))
                    {
                        var data = new Dictionary<string, string> {["text"] = n.InnerText};
                        _l.Debug("Publishing event");
                        _p.PublishEvent(new Message {Data = data});
                    }
                }
                else
                {
                    var data = new Dictionary<string, string> { ["text"] = n.InnerText };
                    _l.Debug("Publishing event");
                    _p.PublishEvent(new Message { Data = data });
                }
            }
        }
    }

    public class WebCheckerOptions
    {
        public WebCheckerOptions()
        {
            Url = "http://ba.no";
            XPath = "//text()[normalize-space(.) != '']";
            Regex = "(?i)brann";
        }

        [Description("Url to website you want to check")]
        public string Url { get; set; }

        [Description("XPath to find a subsection of the site")]
        public string XPath { get; set; }

        [Description("Regex to match with content in subsection")]
        public string Regex { get; set; }
    }
}
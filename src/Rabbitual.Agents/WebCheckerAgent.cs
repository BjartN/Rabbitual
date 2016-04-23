using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    public class WebCheckerAgent : Agent<WebCheckerOptions>,
        IScheduledAgent,
        IPublishingAgent
    {
        private readonly ILogger _l;

        public WebCheckerAgent(ILogger _l)
        {
            this._l = _l;
        }

        public int DefaultSchedule => 4000;

        /// <summary>
        /// Use xpath to match node, and check for text in node using RegEx.
        /// Output the inner text of every matched node as an event.
        /// </summary>
        public void Check()
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
                        Publisher.PublishEvent(new Message {Data = data});
                    }
                }
                else
                {
                    var data = new Dictionary<string, string> { ["text"] = n.InnerText };
                    _l.Debug("Publishing event");
                    Publisher.PublishEvent(new Message { Data = data });
                }
            }
        }

        public WebCheckerOptions Options { get; set; }
        public IPublisher Publisher { get; set; }
        public string Id { get; set; }
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
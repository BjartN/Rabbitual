namespace Rabbitual.Agents.WebServerAgent
{
    [Icon("server")]
    public class WebServerAgent:IAgent
    {
        private readonly WebServer _s;

        public WebServerAgent(WebServer s)
        {
            _s = s;
        }

        public int Id { get; set; }
        public void Start()
        {
            _s.Start();
        }

        public void Stop()
        {
            _s.Stop();
        }
    }
}
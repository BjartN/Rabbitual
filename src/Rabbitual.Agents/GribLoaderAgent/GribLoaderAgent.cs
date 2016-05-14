using System.IO;
using Rabbitual.Core;

namespace Rabbitual.Agents.GribLoaderAgent
{
    public class GribLoaderAgent:StatefulAgent<GribLoaderOptions,GribLoaderState>, IEventConsumerAgent

    {
        public GribLoaderAgent(GribLoaderOptions options, IAgentStateRepository stateRepository) : base(options, stateRepository)
        {
        }

        public void Consume(Message evt)
        {
            string file=null;
            evt.Data.TryGetValue("FilePath", out file);
            if (file == null)
                return;
            if (!File.Exists(file))
                return;
            

        }
    }

    public class GribLoaderState
    {
    }

    public class GribLoaderOptions
    {
    }
}
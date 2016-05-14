using Rabbitual.Core;

namespace Rabbitual.Logging
{
    public interface IAgentMessageLog
    {
        void LogIncoming(Message m);
        void LogOutgoing(Message m);

        Message[] GetIncoming();
        Message[] GetOutGoing();

        LogSummary GetSummary();
    }
}
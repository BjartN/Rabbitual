using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rabbitual.Configuration;
using Rabbitual.Logging;

namespace Rabbitual
{

    /// <summary>
    /// Wrap all access to agents to make sure only one thread is accessing it at the time.
    /// </summary>
    public class AgentWrapper : IAgentWrapper
    {
        private readonly IAgent _agent;
        private readonly IAgentMessageLog _al;
        private readonly ILogger _logger;
        private readonly Dictionary<string, bool> _sourceIdx;
        private readonly BufferBlock<Message> _buffer;

        public AgentWrapper(IAgent agent, AgentConfig config, IAgentMessageLog al, ILogger logger)
        {
            _agent = agent;
            _sourceIdx = config.Sources.ToDictionary(x => x.Id, x => true);
            _al = al;
            _logger = logger;
            Id = _agent.Id;

            Config = config;
            _buffer = setUpBuffer(_agent);
        }

        public AgentConfig Config { get; set; }

        /// <summary>
        /// Ensure that only one thread is accessing the agent at a time
        /// (There is probably a much cleaner way to do this.)
        /// </summary>
        private BufferBlock<Message> setUpBuffer(IAgent agent)
        {
            var b = new BufferBlock<Message>();
            var taskAgent = agent as ITaskConsumerAgent;
            var eventAgent = agent as IEventConsumerAgent;
            var scheduledAgent = agent as IScheduledAgent;

            if (taskAgent != null)
            {
                var doWork = new ActionBlock<Message>(message =>
                {
                    Task.Run(() =>
                    {
                        _logger.Info("{0}: Work start", Id);
                        taskAgent.DoWork(message);
                        _logger.Info("{0}: Work end", Id);
                    });
                });
                b.LinkTo(doWork, m => m.MessageType == MessageType.Task);

            }

            if (eventAgent != null)
            {
                var consume = new ActionBlock<Message>(message =>
                {
                    Task.Run(() =>
                    {
                        _logger.Info("{0}: Consume start", Id);
                        eventAgent.Consume(message);
                        _logger.Info("{0}: Consume end", Id);
                    });

                });
                b.LinkTo(consume, m => m.MessageType == MessageType.Event);

            }

            if (scheduledAgent != null)
            {
                var check = new ActionBlock<Message>(message =>
                {
                    _logger.Info("{0}: Schedule start", Id);
                    scheduledAgent.Check();
                    _logger.Info("{0}: Schedule end", Id);
                });
                b.LinkTo(check, m => m.MessageType == MessageType.Check);
            }

            return b;
        }

        public string Id { get; set; }

        public bool CanConsume(string fromAgentId)
        {
            return _sourceIdx.Count == 0 || _sourceIdx.ContainsKey(fromAgentId);
        }

        public bool IsWorker()
        {
            return _agent is ITaskConsumerAgent;
        }

        public bool CanDoWork(Message message)
        {
            return ((ITaskConsumerAgent)_agent).CanDoWork(message);
        }

        public void DoWork(Message message)
        {
            message.MessageType = MessageType.Task;
            _al.LogIncoming(message);
            _buffer.Post(message);
        }

        public void Consume(Message message)
        {
            message.MessageType = MessageType.Event;
            _al.LogIncoming(message);
            _buffer.Post(message);
        }

        public bool IsScheduled()
        {
            return _agent is IScheduledAgent;
        }

        public int GetSchedule()
        {
            return ((IScheduledAgent)_agent).DefaultScheduleMs;
        }

        public bool IsPublisher()
        {
            return _agent is IEventPublisherAgent;
        }

        public object GetState()
        {
            return StateHelper.GetStateUsingMagic(_agent);
        }

        public void Check()
        {
            var message = new Message { MessageType = MessageType.Check };
            _al.LogIncoming(message);
            _buffer.Post(message);
        }

        public bool IsConsumer()
        {
            return _agent is IEventConsumerAgent;
        }

        public bool HasState()
        {
            return _agent.GetType().IsOfType(typeof(IStatefulAgent<>));
        }

        public void Start()
        {
            _agent.Start();
        }

        public void Stop()
        {
            _agent.Stop();
        }

    }

}
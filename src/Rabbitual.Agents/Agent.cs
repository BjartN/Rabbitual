﻿using Rabbitual.Core;

namespace Rabbitual.Agents
{
    public abstract class Agent<TOptions> : IHaveOptions<TOptions> where TOptions : class
    {
        protected Agent(TOptions options)
        {
            Options = options;
        }

        public int Id { get; set; }
        public virtual void Start() { }
        public virtual void Stop() { }

        public TOptions Options { get; protected set; }
    }
}
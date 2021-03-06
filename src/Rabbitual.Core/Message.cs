﻿using System;
using System.Collections.Generic;

namespace Rabbitual.Core
{
    public class Message
    {
        public Message()
        {
            Occured=DateTime.UtcNow;
            Data = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Data { get; set; }
        public int SourceAgentId { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime Occured { get; set; }
    }

    public enum MessageType
    {
        Task, Event,Check
    }
}

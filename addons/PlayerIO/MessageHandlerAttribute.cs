using System;
using System.Collections;
using System.Collections.Generic;

namespace PIOMP
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MessageHandlerAttribute : Attribute
    {
        /// <summary>The ID of the message type that this method is meant to handle.</summary>
        public string MessageId { get; private set; }
        /// <summary>The ID of the group of message handlers this method belongs to.</summary>

        /// <summary>Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class with the <paramref name="messageId"/> value.</summary>
        /// <param name="messageId">The ID of the message type that this method is meant to handle.</param>
        public MessageHandlerAttribute(string messageId)
        {
            MessageId = messageId;
        }
    }
}
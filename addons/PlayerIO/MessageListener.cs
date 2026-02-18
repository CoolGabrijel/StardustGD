using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PIOMP
{
    /// <summary>
    /// Class for Listening Messages from PlayerIO services and calling Methods
    /// </summary>
    internal static class MessageListener
    {
        public delegate void MessageHandler(Message _msg);

        static Dictionary<string, MessageHandler> messageHandlers;
        static ActionQueue actionQueue = new ActionQueue();

        /// <summary>
        /// Call to execute Methods
        /// </summary>
        public static void Tick() => actionQueue.ExecuteAll();

        internal static void OnMessage(object _sender, Message _msg)
        {
            actionQueue.Add(() =>
            {
                // This block may execute on a different thread, so we double check if the client is still in the dictionary in case they disconnected
                if (Room.IsInRoom)
                {
                    if (messageHandlers.TryGetValue(_msg.Type, out MessageHandler messageHandler))
                    {
                        messageHandler(_msg);
                    }
                }
            });
        }

        public static void CreateMessageHandlers(Assembly _assembly)
        {
            List<MethodInfo> methodList = _assembly.GetTypes()
                                           .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) // Include instance methods in the search so we can show the developer an error instead of silently not adding instance methods to the dictionary
                                           .Where(m => m.GetCustomAttributes(typeof(MessageHandlerAttribute), false).Length > 0)
                                           .ToList();

            // In Unity, PIOMP and the actual assembly were different.
            // In Godot, they are both the same assembly, so we do not need to do this
            // Leaving it commented just in case

            //MethodInfo[] defaultMethods = typeof(DefaultMsgHandle).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            //                                                .Where(m => m.GetCustomAttributes(typeof(MessageHandlerAttribute), false).Length > 0)
            //                                                .ToArray();

            //methodList.AddRange(defaultMethods);
            MethodInfo[] methods = methodList.ToArray();

            messageHandlers = new Dictionary<string, MessageHandler>(methods.Length);
            for (int i = 0; i < methods.Length; i++)
            {
                MessageHandlerAttribute attribute = methods[i].GetCustomAttribute<MessageHandlerAttribute>();

                if (!methods[i].IsStatic)
                    throw new Exception($"Message handler methods should be static, but '{methods[i].DeclaringType}.{methods[i].Name}' is an instance method!");

                Delegate clientMessageHandler = Delegate.CreateDelegate(typeof(MessageHandler), methods[i], false);
                if (clientMessageHandler != null)
                {
                    // It's a message handler for Client instances
                    if (messageHandlers.ContainsKey(attribute.MessageId))
                    {
                        MethodInfo otherMethodWithId = messageHandlers[attribute.MessageId].GetMethodInfo();
                        throw new Exception($"Client-side message handler methods '{methods[i].DeclaringType}.{methods[i].Name}' and '{otherMethodWithId.DeclaringType}.{otherMethodWithId.Name}' are both set to handle messages with ID {attribute.MessageId}! Only one handler method is allowed per message ID!");
                    }
                    else
                        messageHandlers.Add(attribute.MessageId, (MessageHandler)clientMessageHandler);
                }
                else
                {
                    throw new Exception($"Something went wrong with {methods[i].DeclaringType}.{methods[i].Name}");
                }
            }
        }
    }
}
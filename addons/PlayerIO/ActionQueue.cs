using System;
using System.Collections.Generic;

namespace PIOMP
{
    /// <summary>Provides functionality for queueing methods for later execution from a chosen thread.</summary>
    public class ActionQueue
    {
        private readonly List<Action> executionQueue = new List<Action>();
        private readonly List<Action> executionQueueCopy = new List<Action>();
        private bool hasActionToExecute = false;

        /// <summary>Adds an action to the queue.</summary>
        /// <param name="action">The action to be added to the queue.</param>
        public void Add(Action action)
        {
            if (action == null)
            {
                throw new Exception("No Action to execute!");
            }

            lock (executionQueue)
            {
                executionQueue.Add(action);
                hasActionToExecute = true;
            }
        }

        /// <summary>Executes all actions in the queue on the calling thread.</summary>
        /// <remarks>This method should only be called from a single thread in the application.</remarks>
        public void ExecuteAll()
        {
            if (hasActionToExecute)
            {
                executionQueueCopy.Clear();
                lock (executionQueue)
                {
                    executionQueueCopy.AddRange(executionQueue);
                    executionQueue.Clear();
                    hasActionToExecute = false;
                }

                // Execute all actions from the copied queue
                for (int i = 0; i < executionQueueCopy.Count; i++)
                    executionQueueCopy[i]();
            }
        }

        /// <summary>Clears all actions in the queue without executing them.</summary>
        public void Clear()
        {
            lock (executionQueue)
            {
                executionQueue.Clear();
                hasActionToExecute = false;
            }
        }
    }
}
﻿using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Pipeline.Messages;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Used to abstract away the storage handling from the rest of the handlers.
    /// </summary>
    /// <remarks>Will only work properly if all handlers invoke the correct upstream messages which is: <see cref="CommandCompleted"/>, <see cref="CommandFailed"/> and <see cref="CommandAborted"/>.
    /// <para>This handler should probably be the first one in the pipeline since it depends on the messages from other handlers.</para>
    /// </remarks>
    public class StorageHandler : IDownstreamHandler, IUpstreamHandler
    {
        private readonly ICommandStorage _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageHandler" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public StorageHandler(ICommandStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, IDownstreamMessage message)
        {
            var msg = message as DispatchCommand;
            if (msg != null)
            {
                _storage.Add(msg);
            }
            if (message is StartHandlers)
            {
                context.SendDownstream(message);

                var commands = _storage.FindFailedCommands(DateTime.Now.AddSeconds(-5));
                foreach (var command in commands)
                {
                    context.SendDownstream(command);
                }

                return;
            }
            context.SendDownstream(message);
        }

        #endregion

        #region IUpstreamHandler Members

        //TODO: Unit test all upstream messages
        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, IUpstreamMessage message)
        {
            // great. let's remove it
            var msg = message as CommandCompleted;
            if (msg != null)
            {
                _storage.Delete(msg.Message.Command);
            }

            // won't try anymore.
            var aborted = message as CommandAborted;
            if (aborted != null)
            {
                _storage.Delete(aborted.Message.Command);
            }

            var failed = message as CommandFailed;
            if (failed != null)
            {
                _storage.Update(failed.Message);
            }

            context.SendUpstream(message);
        }

        #endregion
    }
}
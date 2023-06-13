﻿using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Client
{
    public interface IPipeClient<TMessage> : IPipe<TMessage>, IPipeEvents<TMessage>, IPipeClientEvents<TMessage>
    {
        /// <summary>The name of pipe.</summary>
        public string PipeName { get; }


        /// <summary>
        /// Checks that connection is exists.
        /// </summary>
        bool IsConnected { get; }


        /// <summary>
        /// Used server name.
        /// </summary>
        public string ServerName { get; }



        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects from server
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}
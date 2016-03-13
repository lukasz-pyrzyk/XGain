﻿using System;
using System.Threading.Tasks;
using XGain.Sockets;

namespace XGain.Processing
{
    public interface IProcessor<in T> : IProcessor where T : Message
    {
        void ProcessSocketConnection(ISocket socket, T args);
    }

    public interface IProcessor
    {
        void ProcessSocketConnection(ISocket socket, EventArgs args);
    }
}

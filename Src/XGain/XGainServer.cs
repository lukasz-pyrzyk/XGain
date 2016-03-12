﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using XGain.Processing;
using XGain.Sockets;

namespace XGain
{
    public class XGainServer : IServer
    {
        public event EventHandler<Message> OnNewMessage;
        private readonly Func<IProcessor> _requestProcessorResolver;
        private readonly TcpListener _listener;

        public XGainServer(IPAddress ipAddress, int port, Func<IProcessor> requestProcessorResolver)
        {
            _requestProcessorResolver = requestProcessorResolver;
            _listener = new TcpListener(ipAddress, port);
        }

        public async Task Start()
        {
            _listener.Start();

            while (true)
            {
                try
                {
                    Socket socket = await _listener.AcceptSocketAsync();
                    ISocket request = new XGainSocket(socket);
                    Task.Factory.StartNew(() =>
                    {
                        ProcessSocketConnection(request);
                    });
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void ProcessSocketConnection(ISocket socket)
        {
            Message args = new Message();

            IProcessor processor = _requestProcessorResolver();
            Task processing = processor.ProcessSocketConnection(socket, args);
            processing.Wait();

            var handler = OnNewMessage;
            handler?.Invoke(socket, args);
        }
        
        public void Dispose()
        {
            try
            {
                _listener.Stop();
            }
            catch (SocketException ex)
            {
            }
        }
    }
}
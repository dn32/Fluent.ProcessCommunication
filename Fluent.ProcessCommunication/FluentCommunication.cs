using System;
using System.IO.Pipes;
using System.Threading;

namespace Fluent
{
    public class ProcessCommunication
    {
        public int BufferSize { get; set; }

        public string PipeName { get; set; }

        public Action<byte[]> Callback { get; set; }

        private byte[] Buffer { get; set; }

        private NamedPipeServerStream ServerStream { get; set; }

        public ProcessCommunication(string pipeName, Action<byte[]> callback, int bufferSize = 1048576)
        {
            this.PipeName = pipeName;
            this.BufferSize = bufferSize;
            this.Callback = callback;

            Buffer = new byte[BufferSize];
            ServerStream = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public ProcessCommunication Init()
        {
            ServerStream.BeginWaitForConnection(this.ConnectionCallback, this);
            return this;
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var pipeServer = (dynamic)ar.AsyncState;

            if (ServerStream.IsConnected)
            {
                if (!ar.IsCompleted)
                {
                    throw new Exception($"Package larger than the system cache: {BufferSize} bytes");
                }

                int received = ServerStream.EndRead(ar);
                var array = new byte[received];
                Array.Copy(pipeServer.Buffer, array, received);

                if (array.Length != 0)
                {
                    ThreadPool.QueueUserWorkItem(_ => { this.Callback(array); });
                }

                RestartConnection();
            }
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (dynamic)ar.AsyncState;
            ServerStream.EndWaitForConnection(ar);
            RestartConnection();
        }

        private void RestartConnection()
        {
            if (ServerStream.IsConnected)
            {
                ServerStream.BeginRead(Buffer, 0, BufferSize, this.ReadCallback, this);
            }
            else
            {
                ServerStream.BeginWaitForConnection(this.ConnectionCallback, this);
            }
        }
    }
}

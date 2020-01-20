using Fluent.ProcessCommunication;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;

namespace Fluent
{
    public class ProcessCommunicationServer
    {
        public int BufferSize { get; set; }

        public string nome_da_rota { get; set; }

        public Action<Package> Callback { get; set; }

        private byte[] Buffer { get; set; }

        private NamedPipeServerStream Stream { get; set; }

        public ProcessCommunicationPost ProcessCommunicationPost { get; set; }

        public bool UseCallback { get; set; }

        public ProcessCommunicationServer(string nome_da_rota, string cliente, Action<Package> callback, int bufferSize = 102400)
        {
            this.nome_da_rota = nome_da_rota;
            this.BufferSize = bufferSize;
            this.Callback = callback;
            ProcessCommunicationPost = new ProcessCommunicationPost(cliente);
            ContentByte = new List<byte>();

            Buffer = new byte[BufferSize];
            Stream = new NamedPipeServerStream(this.nome_da_rota, PipeDirection.In, 100, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public ProcessCommunicationServer Init()
        {
            Stream.BeginWaitForConnection(this.ConnectionCallback, this);
            return this;
        }

        public List<byte> ContentByte { get; set; }

        private void ReadCallback(IAsyncResult ar)
        {
            var pipeServer = (dynamic)ar.AsyncState;

            int received = Stream.EndRead(ar);
            var array = new byte[received];
            Array.Copy(pipeServer.Buffer, array, received);
            pipeServer.Buffer = new byte[BufferSize];

            if (array.Length != 0)
            {
                ContentByte.AddRange(array);
            }

            if (Stream.IsConnected && Stream.IsMessageComplete)
            {
                if (ContentByte.Count != 0)
                {
                    var package = Util.FromByteArray(ContentByte.ToArray()) as Package;
                    ThreadPool.QueueUserWorkItem(_ => Callback(package));
                    Stream.Close();
                    ContentByte.Clear();
                }
            }

            RestartConnection();
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (dynamic)ar.AsyncState;
            Stream.EndWaitForConnection(ar);
            RestartConnection();
        }

        private void RestartConnection()
        {
            if (Stream.IsConnected)
            {
                Stream.BeginRead(Buffer, 0, BufferSize, this.ReadCallback, this);
            }
            else
            {
                Stream = new NamedPipeServerStream(nome_da_rota, PipeDirection.In, 100, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                Stream.BeginWaitForConnection(this.ConnectionCallback, this);
            }
        }
    }

}

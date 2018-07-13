using Fluent.ProcessCommunication;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using static Fluent.ProcessCommunication.ProcessCommunicationPost;

namespace Fluent
{
    public class ProcessCommunicationServer
    {
        public int BufferSize { get; set; }

        public string PipeName { get; set; }

        public Action<Package> Callback { get; set; }

        private byte[] Buffer { get; set; }

        private NamedPipeServerStream Stream { get; set; }

        public ProcessCommunicationPost ProcessCommunicationPost { get; set; }

        private byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public ProcessCommunicationServer(string pipeName, Action<Package> callback, int bufferSize = 1048576)
        {
            this.PipeName = pipeName;
            this.BufferSize = bufferSize;
            this.Callback = callback;
            ProcessCommunicationPost = new ProcessCommunicationPost();

            Buffer = new byte[BufferSize];
            Stream = new NamedPipeServerStream(PipeName, PipeDirection.In, 100, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public ProcessCommunicationServer Init()
        {
            Stream.BeginWaitForConnection(this.ConnectionCallback, this);
            return this;
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var pipeServer = (dynamic)ar.AsyncState;

            if (Stream.IsConnected && Stream.IsMessageComplete)
            {
                int received = Stream.EndRead(ar);
                var array = new byte[received];
                Array.Copy(pipeServer.Buffer, array, received);
                pipeServer.Buffer = new byte[BufferSize];


                if (array.Length != 0)
                {
                    var content = Encoding.UTF8.GetString(array);
                    var package = JsonConvert.DeserializeObject<Package>(content);
                    ThreadPool.QueueUserWorkItem(_ => { this.Callback(package); });
                    Stream.Close();
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
                Stream = new NamedPipeServerStream(PipeName, PipeDirection.In, 100, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                Stream.BeginWaitForConnection(this.ConnectionCallback, this);
            }
        }
    }

}

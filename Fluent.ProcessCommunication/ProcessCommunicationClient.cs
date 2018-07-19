using System;
using System.IO.Pipes;
using System.Text;

namespace Fluent
{
    public class ProcessCommunicationClient
    {
        private NamedPipeClientStream Stream { get; set; }
        public string RouteName { get; set; }
        public ProcessCommunicationClient(string routeName)
        {
            this.RouteName = routeName;
            this.Stream = new NamedPipeClientStream(".", this.RouteName, PipeDirection.Out, PipeOptions.Asynchronous);
        }

        public bool Connect(int timeout = 1000)
        {
            this.Stream.Connect(timeout);
            this.Stream.ReadMode = PipeTransmissionMode.Byte;
            return true;
        }

        public bool Post(string json, bool useCallBack = true)
        {
            if (!this.Stream.IsConnected)
            {
                if (!Connect())
                {
                    return false;
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            this.Stream.BeginWrite(buffer, 0, buffer.Length, this.SendCallback, this.Stream);
            return true;
        }

        private void SendCallback(IAsyncResult iar)
        {
            var pipeStream = (NamedPipeClientStream)iar.AsyncState;
            pipeStream.EndWrite(iar);
        }
    }

}

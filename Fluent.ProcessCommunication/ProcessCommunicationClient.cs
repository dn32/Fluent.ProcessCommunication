using System;
using System.IO.Pipes;
using System.Text;

namespace Fluent
{
    public class ProcessCommunicationClient
    {
        private NamedPipeClientStream Stream { get; set; }
        public string nome_da_rota { get; set; }
        public ProcessCommunicationClient(string nome_da_rota)
        {
            this.nome_da_rota = nome_da_rota;
            this.Stream = new NamedPipeClientStream(".", this.nome_da_rota, PipeDirection.Out, PipeOptions.Asynchronous);
        }

        public bool Connect(int timeout = 1000)
        {
            this.Stream.Connect(timeout);
            this.Stream.ReadMode = PipeTransmissionMode.Byte;
            return true;
        }

        public bool Post(string json)
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

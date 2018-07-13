using Newtonsoft.Json;
using System;
using System.Text;

namespace Fluent.ProcessCommunication.Server.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Servidor iniciando!");

            void rx(byte[] data)
            {
                var json = Encoding.UTF8.GetString(data);
                var pacote = JsonConvert.DeserializeObject<Pacote>(json);
                Console.WriteLine(pacote.Content);
            }

            new ProcessCommunicationServer("cache", rx).Init();

            Console.ReadKey();
        }
    }

    internal class Pacote
    {
        public object Content { get; set; }
        public string Key { get; set; }
    }
}

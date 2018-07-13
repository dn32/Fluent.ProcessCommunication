using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using static Fluent.ProcessCommunication.Test.Program;

namespace Fluent.ProcessCommunication.Test
{
    partial class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Cliente iniciando!");

             var cliente = new Cliente();

            while (true)
            {
                //new Cliente().post();
                //new Cliente().post();
                //new Cliente().post();
                cliente.post();
                cliente.post();
                cliente.post();

                Console.ReadKey();
            }
        }


    }

    public class Cliente
    {
        public Cliente()
        {
            pipeClient = new ProcessCommunicationClient("cache");
        }

        public byte[] ToByteArray<T>(T obj)
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

        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        const bool USAR_JSON = true;

        public ProcessCommunicationClient pipeClient;

        public void post()
        {
            var obj = new Teste();
            obj.Nome = "Marcelo teste";
            obj.Sobrenome = "sdsdsdsdsds ddfdf teste";
            var bytes = USAR_JSON ? JsonConvert.SerializeObject(obj) : (object)ToByteArray(obj);

            var pacote = new Pacote
            {
                Content = bytes,
                Key = "123"
            };

            var json = JsonConvert.SerializeObject(pacote);

            pipeClient.Post(json);
        }
    }


}

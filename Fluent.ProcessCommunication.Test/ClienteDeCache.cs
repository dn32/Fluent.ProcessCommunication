using Newtonsoft.Json;
using System;
using System.Text;

namespace Fluent.ProcessCommunication.Test
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // This is the content I want to send
            var cache = new CacheRecord();
            cache.Id = Guid.NewGuid().ToString();
            cache.Content = "content here";

            while (true)
            {
                var retorno = new ProcessCommunicationPost("test-client").Post<bool>(cache);
                var json = JsonConvert.SerializeObject(retorno);
                Console.WriteLine(json);
                Console.ReadKey();
            }
        }
    }
}

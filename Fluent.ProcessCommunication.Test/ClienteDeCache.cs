using Fluent.ProcessCommunication.Test.Shared;
using System;
using System.Threading.Tasks;

namespace Fluent.ProcessCommunication.Client.Test
{
    partial class Program
    {
        static void Main()
        {
            Task.Delay(1000).Wait();

            // This is the content I want to send
            var cache = new CacheRecord
            {
                Id = Guid.NewGuid().ToString(),
                Content = "content here"
            };

            while (true)
            {
                var retorno = new ProcessCommunicationPost("test-client").Post(cache, 30000);
                Console.WriteLine(retorno.Content);
                Console.ReadKey();
            }
        }
    }
}

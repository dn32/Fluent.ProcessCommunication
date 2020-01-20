using Fluent.ProcessCommunication.Test.Shared;
using System;

namespace Fluent.ProcessCommunication.Server.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started!");

            ProcessCommunicationServer process = null;

            void callback(Package package)
            {
                var obj = package.Content as CacheRecord;
                Console.WriteLine(obj.Content);

                package.Content = "Return ok";

                process.ProcessCommunicationPost.Response(package.TransportKey, package);
            }

            var client = "test-client";

            process = new ProcessCommunicationServer($"post-{client}", client, callback);
            process.Init();

            Console.ReadKey();
        }
    }
}

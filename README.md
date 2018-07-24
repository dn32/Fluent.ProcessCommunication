# Fluent.ProcessCommunication

#### Server operations

```C++
using Fluent.ProcessCommunication.Test;
using Newtonsoft.Json;
using System;

namespace Fluent.ProcessCommunication.Server.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started!");

            ProcessCommunicationServer process = null;

            // callback is called whenever the client sends a message to the server
            void callback(Package package)
            {
                // content is the content of the message sent by the client
                var content = JsonConvert.DeserializeObject<CacheRecord>((string)package.Content);
                Console.WriteLine(content.Content);

                // Here you treat what you want when you receive the request
                var ret = true;

                // You deliver the customer feedback here
                process.ProcessCommunicationPost.Response(package.TransportKey, ret);
            }

            var client = "test-client";

            process = new ProcessCommunicationServer($"post-{client}", client, callback);
            process.Init();

            Console.ReadKey();
        }
    }
}
```

-----------------------------------

#### Client operations

```C++
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

            var retorno = new ProcessCommunicationPost("test-client").Post<bool>(cache);
            var json = JsonConvert.SerializeObject(retorno);
            Console.WriteLine(json);
            Console.ReadKey();
        }
    }
}
```

-----------------------------------

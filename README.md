# Fluent.ProcessCommunication

#### Server operations

```C++
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server started!");

            ProcessCommunicationServer process = null;

            // callback is called whenever the client sends a message to the server
            void callback(Package package)
            {
                // content is the content of the message sent by the client
                var content = JsonConvert.DeserializeObject<CadastroDeCache>((string)package.Content);

                // Here you treat what you want when you receive the request
                var obj = new RetornoDeCadastroDeCache();
                obj.Conteudo = content.Conteudo;
                obj.Status = "ok";


                // You deliver the customer feedback here
                process.ProcessCommunicationPost.Response(package.TransportKey, obj);
            }

            var cliente = "test-client";

            process = new ProcessCommunicationServer($"post-{cliente}", cliente,  callback);
            process.Init();

            Console.ReadKey();
        }
    }

```

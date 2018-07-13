using Newtonsoft.Json;
using System;

namespace Fluent.ProcessCommunication.Server.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Servidor iniciando!");

            ProcessCommunicationServer process = null;

            void callback(Package pacote)
            {
                var conteudo = JsonConvert.DeserializeObject<CadastroDeCache>((string)pacote.Content);
                
                // - ARQUI É FEITO O TRATAMENTO DA OPERAÇÃO SOLICITADA
                var obj = new RetornoDeCadastroDeCache();
                obj.Identificador = conteudo.Identificador;
                obj.Status = "ok";

                // - AQUI É ENTREGUE A RESPOSTA PARA O SOLICITANTE
                process.ProcessCommunicationPost.Response(pacote.TransportKey, obj);
            }

            process = new ProcessCommunicationServer("post", callback);
            process.Init();

            Console.ReadKey();
        }
    }
}

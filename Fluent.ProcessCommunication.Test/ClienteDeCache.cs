using Newtonsoft.Json;
using System;

namespace Fluent.ProcessCommunication.Test
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var cache = new CadastroDeCache();
            cache.Identificador = Guid.NewGuid().ToString();
            cache.Conteudo = "Conteúdo do cache";

            while (true)
            {
                var retorno = new ProcessCommunicationPost().Post<RetornoDeCadastroDeCache>(cache);
                var json = JsonConvert.SerializeObject(retorno);
                Console.WriteLine(json);
                Console.ReadKey();
            }
        }
    }
}

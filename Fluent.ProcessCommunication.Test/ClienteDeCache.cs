using Newtonsoft.Json;
using System;
using System.Text;

namespace Fluent.ProcessCommunication.Test
{
    partial class Program
    {
        public static string Repeat(string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        static void Main(string[] args)
        {
            var cache = new CadastroDeCache();
            cache.Identificador = Guid.NewGuid().ToString();
            cache.Conteudo = Repeat("123456789-", 300000) + "\nAAAAAAA";

            while (true)
            {
                var retorno = new ProcessCommunicationPost("Cliente-de-teste").Post<bool>(cache);
                var json = JsonConvert.SerializeObject(retorno);
                Console.WriteLine(json);
                Console.ReadKey();
            }
        }
    }
}

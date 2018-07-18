using System;

namespace Fluent.ProcessCommunication.Server.Test
{
    [Serializable]
    public class RetornoDeCadastroDeCache
    {
        public string Identificador { get; set; }
        public string Status { get; set; }
        public object Conteudo { get; internal set; }
    }
}

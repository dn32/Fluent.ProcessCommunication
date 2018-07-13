using System;

namespace Fluent.ProcessCommunication.Test
{
    [Serializable]
    public class CadastroDeCache
    {
        public string Identificador { get; set; }
        public object Conteudo { get; set; }
    }
}

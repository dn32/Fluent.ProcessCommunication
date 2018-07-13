using System;

namespace Fluent.ProcessCommunication.Test
{

    partial class Program
    {
        [Serializable]
        public class RetornoDeCadastroDeCache
        {
            public string Identificador { get; set; }
            public string Status { get; set; }
        }
    }
}

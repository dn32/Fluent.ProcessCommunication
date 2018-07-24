using System;

namespace Fluent.ProcessCommunication.Test
{
    [Serializable]
    public class CacheRecord
    {
        public string Id { get; set; }
        public object Content { get; set; }
    }
}

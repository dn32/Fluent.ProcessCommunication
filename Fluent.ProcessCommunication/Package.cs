using System;

namespace Fluent.ProcessCommunication
{
    [Serializable]
    public class Package
    {
        public object Content { get; set; }
        public string TransportKey { get; set; }
        public bool UseResponse { get; set; }
        public string AditionalInformation { get; set; }
    }
}

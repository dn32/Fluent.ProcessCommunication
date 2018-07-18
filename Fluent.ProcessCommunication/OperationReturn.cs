using System;
using System.Collections.Generic;
using System.Text;

namespace Fluent.ProcessCommunication
{
    public class OperationReturn
    {
        public OperationReturn(eOperationReturn result)
        {
            Result = result;
        }

        public eOperationReturn Result { get; set; }
    }
}

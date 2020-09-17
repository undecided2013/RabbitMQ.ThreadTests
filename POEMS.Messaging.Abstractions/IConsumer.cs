using System;
using System.Collections.Generic;
using System.Text;

namespace POEMS.Messaging.Abstractions
{
    public interface IConsumer
    {
        void ReceiveAsync(Func<byte[], bool> processorMethod);
    }
}

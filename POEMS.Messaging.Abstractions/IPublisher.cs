using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POEMS.Messaging.Abstractions
{
    public interface IPublisher
    {
        PublishResult Publish(byte[] message);
    }
}

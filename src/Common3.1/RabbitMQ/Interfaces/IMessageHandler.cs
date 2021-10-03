using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMQ
{
    public interface IMessageHandler
    {
        void Start(IMessageHandlerCallback callback);
        void Stop();
    }
}

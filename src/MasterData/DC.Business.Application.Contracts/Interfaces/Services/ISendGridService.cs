using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Application.Contracts.Interfaces.Services
{
    public interface ISendGridService
    {
        Task SendMessageUserCreated(string destination);
        Task SendMessagePropertyCreated(string destination);
    }
}

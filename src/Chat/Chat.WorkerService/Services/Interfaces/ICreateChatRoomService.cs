using Chat.WorkerService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.WorkerService.Services.Interfaces
{
    public interface ICreateChatRoomService
    {
        Task Handle(EmailDto email);
    }
}

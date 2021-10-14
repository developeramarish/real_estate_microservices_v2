using Chat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Infrastructure.IRepositories.Interfaces
{
    public interface IChatRoomRepository
    {
        Task<ChatRoom> GetByIdAsync(string id);
        Task<List<ChatRoom>> GetByIdsAsync(string[] ids);
        Task CreateAsync(ChatRoom chatRoom);
        Task UpdateMessagesAsync(ChatRoom chatRoom);
        Task DeleteAsync(string id);
    }
}

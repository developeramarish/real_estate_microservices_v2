using Chat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Infrastructure.IRepositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByMySqlIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task CreateAsync(User user);
        Task UpdateAddChatRoomAsync(User user);
        Task DeleteAsync(string id);
        Task UpdateConnectionIdsAsync(User user);
    }
}

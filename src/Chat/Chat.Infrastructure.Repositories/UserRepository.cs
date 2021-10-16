using Chat.Domain;
using Chat.Domain.Entities;
using Chat.Infrastructure.IRepositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;
        private readonly MongoConnectionAppSettings _settings;
        private readonly string CollectionName = "User";
        public UserRepository(IOptions<MongoConnectionAppSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _collection = database.GetCollection<User>(CollectionName);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _collection.Find<User>(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByMySqlIdAsync(int id)
        {
            return await _collection.Find<User>(c => c.MySqlId == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _collection.Find<User>(c => c.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _collection.InsertOneAsync(user);
        }
        public async Task UpdateAddChatRoomAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            var update = Builders<User>.Update
                .Set(s => s.ChatRoomIds, user.ChatRoomIds);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateConnectionIdsAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            var update = Builders<User>.Update
                .Set(s => s.SignalRConnectionIds, user.SignalRConnectionIds);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }
    }
}

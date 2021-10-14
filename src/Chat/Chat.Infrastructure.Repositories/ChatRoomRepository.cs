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
    public class ChatRoomRepository : IChatRoomRepository
    {
        private readonly IMongoCollection<ChatRoom> _collection;
        private readonly MongoConnectionAppSettings _settings;
        private readonly string CollectionName = "ChatRoom";
        public ChatRoomRepository(IOptions<MongoConnectionAppSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _collection = database.GetCollection<ChatRoom>(CollectionName);
        }

        public async Task<ChatRoom> GetByIdAsync(string id)
        {
            return await _collection.Find<ChatRoom>(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ChatRoom>> GetByIdsAsync(string[] ids)
        {
            var filter = Builders<ChatRoom>.Filter.In(x => x.Id, ids);
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task CreateAsync(ChatRoom chatRoom)
        {
            await _collection.InsertOneAsync(chatRoom);
        }
        public async Task UpdateMessagesAsync(ChatRoom chatRoom)
        {
            var filter = Builders<ChatRoom>.Filter.Eq(s => s.Id, chatRoom.Id);
            var update = Builders<ChatRoom>.Update
                .Set(s => s.Messages, chatRoom.Messages)
                .Set(x => x.Updated, DateTime.UtcNow);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }
    }
}

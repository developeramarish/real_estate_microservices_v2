using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat.Domain.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        public int? MySqlId { get; private set; }
        public string Email { get; private set; }
        public List<string> ChatRoomIds { get; private set; }
        public List<string> SignalRConnectionIds { get; private set; }

        public DateTime Created;
        public DateTime Updated;

        public void Create(int? mySqlId, string email)
        {
            MySqlId = mySqlId;
            Email = email;
            ChatRoomIds = new List<string>();
            SignalRConnectionIds = new List<string>();
            Created = DateTime.UtcNow;
        }

        private void UpdateEmail(string email)
        {
            Email = email;
            Updated = DateTime.UtcNow;
        }

        public void AddChat(string chatId)
        {
            ChatRoomIds.Add(chatId);
            Updated = DateTime.UtcNow;
        }

        public void RemoveChat(string chatId)
        {
            var itemToRemove = ChatRoomIds.Single(r => r.Contains(chatId));
            ChatRoomIds.Remove(itemToRemove);
            Updated = DateTime.UtcNow;
        }

        public void AddConnectionId(string connectionId)
        {
            SignalRConnectionIds.Add(connectionId);
            Updated = DateTime.UtcNow;
        }

        public void RemoveConnectionId(string connectionId)
        {
            var itemToRemove = SignalRConnectionIds.Single(r => r.Contains(connectionId));
            SignalRConnectionIds.Remove(itemToRemove);
            Updated = DateTime.UtcNow;
        }
    }


}

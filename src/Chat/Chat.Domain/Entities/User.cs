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
        public string Id { get; set; }
        public int MySqlId { get; set; }
        public List<string> ChatRoomIds { get; set; }

        public DateTime Created;
        public DateTime Updated;

        public void Create(int mySqlId)
        {
            MySqlId = mySqlId;
            ChatRoomIds = new List<string>();
            Created = DateTime.UtcNow;
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
    }


}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Domain.Entities
{
    public class ChatRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        public int FirstUserMySqlId { get; private set; }
        public int SecondUserMySqlId { get; private set; }
        public List<Message> Messages { get; private set; }
        public DateTime Updated { get; private set; }

        public void Create(int firstUserId, int secondUserId)
        {
            FirstUserMySqlId = firstUserId;
            SecondUserMySqlId = secondUserId;
            Messages = new List<Message>();
            Updated = DateTime.UtcNow;
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
            Updated = DateTime.UtcNow;
        }
    }

}

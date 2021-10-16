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
        public int? FirstUserMySqlId { get; private set; }
        public int? SecondUserMySqlId { get; private set; }
        public string FirstUserEmail { get; set; }
        public string SecondUserEmail { get; set; }
        public List<Message> Messages { get; private set; }
        public DateTime Updated { get; private set; }

        public void Create(int? firstUserId, int? secondUserId, string firstUserEmail, string secondUserEmail)
        {
            FirstUserMySqlId = firstUserId;
            SecondUserMySqlId = secondUserId;
            FirstUserEmail = firstUserEmail;
            SecondUserEmail = secondUserEmail;
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

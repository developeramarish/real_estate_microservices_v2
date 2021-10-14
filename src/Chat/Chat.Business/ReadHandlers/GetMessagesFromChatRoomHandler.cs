using Chat.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Business.ReadHandlers
{
    public class GetMessagesFromChatRoomHandler : IRequestHandler<ReadMessagesFromChatRoomRequest, ChatRoom>
    {
        public GetMessagesFromChatRoomHandler()
        {

        }

        public Task<ChatRoom> Handle(ReadMessagesFromChatRoomRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class ReadMessagesFromChatRoomRequest : IRequest<ChatRoom>
    {

    }
}

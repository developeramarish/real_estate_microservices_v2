using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Chat.Domain.Entities;
using Chat.Infrastructure.IRepositories.Interfaces;
using MediatR;

namespace Chat.Business.CommandHadlers
{
    public class AddMessageToChatRoomHandler : IRequestHandler<AddMessageToChatRoomCommand, Unit>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        public AddMessageToChatRoomHandler(IChatRoomRepository chatRoomRepository)
        {
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<Unit> Handle(AddMessageToChatRoomCommand request, CancellationToken cancellationToken)
        {
            Guard.Against.NullOrEmpty(request.ChatRoomId, nameof(request.ChatRoomId));
            Guard.Against.Null(request.Message, nameof(request.Message));

            var chatRoom = await _chatRoomRepository.GetByIdAsync(request.ChatRoomId);
            Guard.Against.Null(chatRoom, nameof(chatRoom));

            chatRoom.AddMessage(request.Message);

            await _chatRoomRepository.UpdateMessagesAsync(chatRoom);

            return Unit.Value;
        }
    }

    public class AddMessageToChatRoomCommand : IRequest<Unit>
    {
        public string ChatRoomId { get; set; }
        public Message Message { get; set; }
    }
}

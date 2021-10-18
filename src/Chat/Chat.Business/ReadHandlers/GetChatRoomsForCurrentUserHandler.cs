using Ardalis.GuardClauses;
using Chat.Domain.Entities;
using Chat.Infrastructure.IRepositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Business.ReadHandlers
{
    public class GetChatRoomsForCurrentUserHandler : IRequestHandler<GetChatRoomsForCurrentUserRequest, List<ChatRoom>>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        public GetChatRoomsForCurrentUserHandler(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
        }

        public async Task<List<ChatRoom>> Handle(GetChatRoomsForCurrentUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.NegativeOrZero(request.MySqlId, nameof(request.MySqlId));

            var user = await _userRepository.GetByMySqlIdAsync(request.MySqlId);

            List<ChatRoom> list = new List<ChatRoom>();
            if (user != null)
            {
                list = await _chatRoomRepository.GetByIdsAsync(user.ChatRoomIds.ToArray());
            }

            return list;
        }
    }

    public class GetChatRoomsForCurrentUserRequest : IRequest<List<ChatRoom>>
    {
        public int MySqlId { get; set; }
    }
}

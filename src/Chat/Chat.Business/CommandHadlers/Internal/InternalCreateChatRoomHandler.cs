//using Ardalis.GuardClauses;
//using Chat.Domain.Entities;
//using Chat.Infrastructure.IRepositories.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Chat.Business.CommandHadlers.Internal
//{
//    public class InternalCreateChatRoomHandler : IRequestHandler<InternalCreateChatRoomCommand, Unit>
//    {
//        private readonly IChatRoomRepository _chatRoomRepository;
//        private readonly IUserRepository _userRepository;

//        public InternalCreateChatRoomHandler(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
//        {
//            _chatRoomRepository = chatRoomRepository;
//            _userRepository = userRepository;
//        }

//        public async Task<Unit> Handle(InternalCreateChatRoomCommand request, CancellationToken cancellationToken)
//        {
//            Guard.Against.NegativeOrZero(request.FirstUserMySqlId, nameof(request.FirstUserMySqlId));
//            Guard.Against.NegativeOrZero(request.SecondUserMySqlId, nameof(request.SecondUserMySqlId));

//            var firstUser = await _userRepository.GetByMySqlIdAsync(request.FirstUserMySqlId);

//            if(firstUser == null)
//            {
//                var newUser = new User();
//                newUser.Create(request.FirstUserMySqlId);
//                await _userRepository.CreateAsync(newUser);
//            }
//            var secondUser = await _userRepository.GetByMySqlIdAsync(request.SecondUserMySqlId);

//            if (secondUser == null)
//            {
//                var newUser = new User();
//                newUser.Create(request.SecondUserMySqlId);
//                await _userRepository.CreateAsync(newUser);
//            }
//            var newChatRoom = new ChatRoom();

//            newChatRoom.Create(firstUser.MySqlId, secondUser.MySqlId);
//            await _chatRoomRepository.CreateAsync(newChatRoom);

//            return Unit.Value;
//        }
//    }

//    public class InternalCreateChatRoomCommand : IRequest<Unit>
//    {
//        public int FirstUserMySqlId { get; set; }
//        public int SecondUserMySqlId { get; set; }
//    }
//}

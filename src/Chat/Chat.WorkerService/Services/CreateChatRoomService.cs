using Ardalis.GuardClauses;
using Chat.Domain.Entities;
using Chat.Infrastructure.IRepositories.Interfaces;
using Chat.WorkerService.Dto;
using Chat.WorkerService.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Chat.WorkerService.Services
{
    public class CreateChatRoomService : ICreateChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IUserRepository _userRepository;
        public CreateChatRoomService(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
        }

        public async Task Handle(EmailDto email)
        {
            Guard.Against.NullOrEmpty(email.FirstUserEmail, nameof(email.FirstUserId));
            Guard.Against.Null(email.SecondUserId, nameof(email.SecondUserId));

            var firstUser = await _userRepository.GetByEmailAsync(email.FirstUserEmail);

            if (firstUser == null)
            {
                firstUser = new User();
                firstUser.Create(email.FirstUserId, email.FirstUserEmail);
                await _userRepository.CreateAsync(firstUser);
            }
            var secondUser = await _userRepository.GetByEmailAsync(email.SecondUserEmail);

            if (secondUser == null)
            {
                secondUser = new User();
                secondUser.Create(email.SecondUserId, email.SecondUserEmail);
                await _userRepository.CreateAsync(secondUser);
            }
            var newChatRoom = new ChatRoom();

            newChatRoom.Create(firstUser.MySqlId, secondUser.MySqlId, firstUser.Email, secondUser.Email);

            var message = new Message();
            message.Created = DateTime.UtcNow;
            message.MessageText = email.Message;

            newChatRoom.AddMessage(message);

            await _chatRoomRepository.CreateAsync(newChatRoom);
        }
    }
}

using Ardalis.GuardClauses;
using Common.RabbitMQ;
using DC.Business.Application.Contracts.Dtos.Organization.Email;
using DC.Business.Application.Contracts.Interfaces.Organization.Email;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DC.Business.Application.Services.Organization.Email
{
    public class SendEmailService : BusinessService<EmailInputDto, VoidOutputDto>, ISendEmailService
    {
        private readonly IRabbitMQClient _rabbitMqClient;
        private readonly IUserRepository _userRepository;

        public SendEmailService(IRabbitMQClient rabbitMqClient, IUserRepository userRepository)
        {
            _rabbitMqClient = rabbitMqClient ?? throw new ArgumentNullException(nameof(rabbitMqClient));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        protected async override Task<OperationResultDto<VoidOutputDto>> ExecuteAsync(EmailInputDto inputDto, CancellationToken cancellationToken = default)
        {
            Guard.Against.NullOrEmpty(inputDto.FirstUserEmail, nameof(inputDto.FirstUserEmail));
            //Guard.Against.NullOrEmpty(inputDto.SecondUserEmail, nameof(inputDto.SecondUserEmail));
            Guard.Against.NegativeOrZero(inputDto.SecondUserId, nameof(inputDto.SecondUserEmail));
            Guard.Against.NullOrEmpty(inputDto.Message, nameof(inputDto.Message));

            if(inputDto.FirstUserId == null)
            {
                var user = await _userRepository.GetUserByEmailAsync(inputDto.FirstUserEmail);
                inputDto.FirstUserId = user == null ? null : (int)user?.Id;
            }

            var seconduser = await _userRepository.GetUserByIdAsync(inputDto.SecondUserId);
            inputDto.SecondUserEmail = seconduser.Email;

            var payloadChat = JsonSerializer.Serialize(new {
                Message = inputDto.Message, 
                FirstUserId = inputDto.FirstUserId,
                SecondUserId = inputDto.SecondUserId,
                FirstUserEmail = inputDto.FirstUserEmail,
                SecondUserEmail = inputDto.SecondUserEmail
            });

            var payloadEmail = JsonSerializer.Serialize(new
            {
                Email = inputDto.SecondUserEmail
            });

            var tasks = new List<Task>();
            
            tasks.Add(_rabbitMqClient.PublishMessageAsync("EmailSent", payloadChat, "chat_key"));
            tasks.Add(_rabbitMqClient.PublishMessageAsync("EmailSent", payloadEmail, "email_key"));

            await Task.WhenAll(tasks);

            return BuildOperationResultDto(new VoidOutputDto());
        }
    }
}

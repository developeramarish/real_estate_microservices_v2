using Ardalis.GuardClauses;
using Common.RabbitMQ;
using DC.Business.Application.Contracts.Dtos.Organization.Email;
using DC.Business.Application.Contracts.Interfaces.Organization.Email;
using DC.Business.Application.Services.Pipeline;
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

        public SendEmailService(IRabbitMQClient rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient ?? throw new ArgumentNullException(nameof(rabbitMqClient));
        }

        protected async override Task<OperationResultDto<VoidOutputDto>> ExecuteAsync(EmailInputDto inputDto, CancellationToken cancellationToken = default)
        {
            Guard.Against.NegativeOrZero(inputDto.FirstUserId, nameof(inputDto.FirstUserId));
            Guard.Against.NegativeOrZero(inputDto.SecondUserId, nameof(inputDto.SecondUserId));
            Guard.Against.NullOrEmpty(inputDto.FirstUserEmail, nameof(inputDto.FirstUserEmail));
            Guard.Against.NullOrEmpty(inputDto.SecondUserEmail, nameof(inputDto.SecondUserEmail));
            Guard.Against.NullOrEmpty(inputDto.Message, nameof(inputDto.Message));

            var payloadChat = JsonSerializer.Serialize(new {
                Message = inputDto.Message, 
                FirstUserId = inputDto.FirstUserId,
                SecondUserId = inputDto.SecondUserId
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

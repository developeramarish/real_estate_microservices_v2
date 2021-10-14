using Common.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.WorkerService.Dto;
using System.Text.Json;
using Chat.WorkerService.Services.Interfaces;

namespace Chat.WorkerService
{
    public class Worker : IHostedService, IMessageHandlerCallback
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;
        IMessageHandler _messageHandler;
        private readonly ICreateChatRoomService _createChatRoomService;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IMessageHandler messageHandler,
            ICreateChatRoomService createChatRoomService)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _configuration = configuration;
            _createChatRoomService = createChatRoomService ?? throw new ArgumentNullException(nameof(createChatRoomService));
        }

        public async Task<bool> HandleMessageAsync(string messageType, string message)
        {
            try
            {
                _logger.LogInformation($"Chat:Received this email: [{message}].");
                var emailIcomingMessage = JsonSerializer.Deserialize<EmailDto>(message);
                _logger.LogInformation($"Chat [{emailIcomingMessage.FirstUserId}].");
                _logger.LogInformation($"Chat [{emailIcomingMessage.SecondUserId}].");
                _logger.LogInformation($"Chat [{emailIcomingMessage.Message}].");
                await _createChatRoomService.Handle(emailIcomingMessage);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while handling event.");
            }

            return true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Start(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _messageHandler.Stop();
            return Task.CompletedTask;
        }
    }
}

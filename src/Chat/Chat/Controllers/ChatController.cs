using Chat.Business.ReadHandlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IMediator _mediator;
        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("mySqlId")]
        public async Task<IActionResult> GetChatRoomsForUserAsync(int mySqlId)
        {
            var result = await _mediator.Send(new GetChatRoomsForCurrentUserRequest() { MySqlId = mySqlId });
            return Ok(result);
        }
    }
}

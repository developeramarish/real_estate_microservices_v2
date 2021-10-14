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
    public class MessageController : ControllerBase
    {
        public MessageController()
        {

        }

        [HttpGet("")]
        public async Task<IActionResult> GetChatRoomsForUserAsync()
        {
            var result = await _getHomePageService.ExecuteServiceAsync(new VoidInputDto());
            return Ok(result);
        }
    }
}

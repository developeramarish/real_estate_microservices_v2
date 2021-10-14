using DC.Business.Application.Contracts.Dtos.Organization.Email;
using DC.Business.Application.Contracts.Interfaces.Organization.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Business.WebApi.Controllers.Organization
{
    [Route("api/organization/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ISendEmailService _sendEmailService;
        public EmailController(ISendEmailService sendEmailService)
        {
            _sendEmailService = sendEmailService ?? throw new ArgumentNullException(nameof(sendEmailService));
        }

        [HttpPost("")]
        public async Task<IActionResult> PostEmail(EmailInputDto email)
        {
            var result = await _sendEmailService.ExecuteServiceAsync(email);

            return Ok(result);
        }
    }
}

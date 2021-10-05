using DC.Business.Application.Contracts.Interfaces.Organization.Home;
using DC.Core.Contracts.Application.Pipeline.Dtos.Input;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Business.WebApi.Controllers.Organization
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IGetHomePageService _getHomePageService;
        public HomeController(IGetHomePageService getHomePageService)
        {
            _getHomePageService = getHomePageService ?? throw new ArgumentNullException(nameof(getHomePageService));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetHomePageAsync()
        {
            var result = await _getHomePageService.ExecuteServiceAsync(new VoidInputDto());
            return Ok(result);
        }
    }
}

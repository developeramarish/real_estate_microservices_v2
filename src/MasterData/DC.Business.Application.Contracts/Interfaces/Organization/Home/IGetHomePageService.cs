using DC.Business.Application.Contracts.Dtos.Organization.Home;
using DC.Core.Contracts.Application.Pipeline;
using DC.Core.Contracts.Application.Pipeline.Dtos.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Interfaces.Organization.Home
{
    public interface IGetHomePageService : IApplicationService<VoidInputDto, HomePageDto>
    {
    }
}

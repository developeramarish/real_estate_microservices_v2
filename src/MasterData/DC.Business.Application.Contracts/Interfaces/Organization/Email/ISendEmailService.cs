using DC.Business.Application.Contracts.Dtos.Organization.Email;
using DC.Core.Contracts.Application.Pipeline;
using DC.Core.Contracts.Application.Pipeline.Dtos.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Application.Contracts.Interfaces.Organization.Email
{
    public interface ISendEmailService : IApplicationService<EmailInputDto, VoidOutputDto>
    {
    }
}

using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Core.Contracts.Application.Pipeline;
using DC.Core.Contracts.Application.Pipeline.Dtos.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Interfaces.Organization.Listing
{
    public interface ITempListingForUnauthenticatedService : IApplicationService<TempPropertyInputDto, ulong>
    {
    }
}

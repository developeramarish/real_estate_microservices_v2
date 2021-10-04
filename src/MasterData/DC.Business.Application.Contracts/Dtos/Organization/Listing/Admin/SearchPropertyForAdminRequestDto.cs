using DC.Business.Application.Contracts.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Dtos.Organization.Listing.Admin
{
    public class SearchPropertyForAdminRequestDto
    {
       public PropertyStateEnumDto Type { get; set; }
    }
}

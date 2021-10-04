using DC.Business.Application.Contracts.Dtos.Enums;
using System;
namespace DC.Business.Application.Contracts.Dtos.Organization.Listing
{
    public class PropertyTypeDto
    {
        public long Id { get; set; }
        public PropertyTypeEnumDto Type { get; set; }
        public string TypeName { get; set; }
    }
}

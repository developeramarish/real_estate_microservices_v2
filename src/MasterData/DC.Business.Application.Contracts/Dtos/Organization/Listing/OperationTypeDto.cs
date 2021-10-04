using DC.Business.Application.Contracts.Dtos.Enums;
using System;
namespace DC.Business.Application.Contracts.Dtos.Organization.Listing
{
    public class OperationTypeDto
    {
        public long Id { get; set; }
        public OperationTypeEnumDto Type { get; set; }
        public string TypeName { get; set; }

    }
}

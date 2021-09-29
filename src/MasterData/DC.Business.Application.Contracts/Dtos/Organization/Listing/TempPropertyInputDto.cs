using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Dtos.Organization.Listing
{
    public class TempPropertyInputDto
    {
        public Guid Id { get; set; }
        public SellHouseDto Property { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Dtos.Organization.Listing
{
    public class CharacteristicsDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public int? CountNumber { get; set; }
        public string IconName { get; set; }
        public bool? Deleted { get; private set; }
    }
}

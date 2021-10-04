using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Business.Application.Contracts.Dtos.Organization.Home
{
    public class HomePageDto
    {
        public List<PropertyBasicDto> HotPropertiesToBuy { get; set; } // Promoted in future
        public List<PropertyBasicDto> NewRoomsToRent { get; set; }
        public List<PropertyBasicDto> NewApartmentsToBuy { get; set; }
    }
}

using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Domain.Entities.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Business.WebApi.Infrastructure.Automapper
{
    public class PropertyTypeProfile : Profile
    {
        public PropertyTypeProfile()
        {
            CreateMap<PropertyType, PropertyTypeDto>();
        }
    }
}

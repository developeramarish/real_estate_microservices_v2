using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Enums;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Enums;

namespace DC.Business.WebApi.Infrastructure.Automapper
{
    public class PropertyTypeProfile : Profile
    {
        public PropertyTypeProfile()
        {
            CreateMap<PropertyType, PropertyTypeDto>();
            CreateMap<PropertyTypeEnum, PropertyTypeEnumDto>();
            CreateMap<PropertyTypeEnumDto, PropertyTypeEnum>();
        }
    }
}

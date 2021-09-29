using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Image;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Domain.Entities.Organization;

namespace DC.Business.WebApi.Infrastructure.Automapper
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<Domain.ElasticEntities.Property, PropertyBasicDto>();
            CreateMap<Property, PropertyBasicDto>();

            CreateMap<Domain.ElasticEntities.Property, PropertySellDto>(); 

            CreateMap<Domain.ElasticEntities.PropertyImage, PropertyImageDto>();
            CreateMap<PropertyImageDto, Domain.ElasticEntities.PropertyImage>();

            CreateMap<CharacteristicsDto, Domain.ElasticEntities.Characteristics>();
            CreateMap<Domain.ElasticEntities.Characteristics, CharacteristicsDto>();

            CreateMap<Characteristics, Domain.ElasticEntities.Characteristics>();
            CreateMap<Characteristics, CharacteristicsDto>();
            CreateMap<Property, SellHouseDto>();
        }
    }
}

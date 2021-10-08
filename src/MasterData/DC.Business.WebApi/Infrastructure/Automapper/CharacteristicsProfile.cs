using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Domain.Entities.Organization;

namespace DC.Business.WebApi.Infrastructure.Automapper
{
    public class CharacteristicsProfile : Profile
    {
            public CharacteristicsProfile()
            {
                CreateMap<Characteristics, CharacteristicsDto>();
                CreateMap<CharacteristicsDto, Characteristics>();
                CreateMap<CharacteristicsDto, Domain.ElasticEntities.Characteristics>();
                CreateMap<Domain.ElasticEntities.Characteristics, CharacteristicsDto>();
                CreateMap<Characteristics, Domain.ElasticEntities.Characteristics>();
        }
    }
}

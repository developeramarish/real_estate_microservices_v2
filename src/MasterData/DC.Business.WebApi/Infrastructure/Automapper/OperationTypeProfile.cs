using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Domain.Entities.Organization;

namespace DC.Business.WebApi.Infrastructure.Automapper
{
    public class OperationTypeProfile : Profile
    {
        public OperationTypeProfile()
        {
            CreateMap<OperationType, OperationTypeDto>();
        }
    }
}

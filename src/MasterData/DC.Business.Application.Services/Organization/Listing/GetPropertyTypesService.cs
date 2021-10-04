using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Listing;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;

namespace DC.Business.Application.Services.Organization.Listing
{
    public class GetPropertyTypesService : BusinessService<string, IEnumerable<PropertyTypeDto>>, IGetPropertyTypesService
    {
        private readonly IListingRepository _listingRepository;
        private IMapper _mapper;

        public GetPropertyTypesService(IListingRepository listingRepository, IMapper mapper)
        {
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<OperationResultDto<IEnumerable<PropertyTypeDto>>> ExecuteAsync(string inputDto, CancellationToken cancellationToken = default)
        {
            var result = await _listingRepository.GetPropertyTypes();

            var mappedResult = _mapper.Map<List<PropertyTypeDto>>(result);

            return BuildOperationResultDto(mappedResult);
        }
    }
}

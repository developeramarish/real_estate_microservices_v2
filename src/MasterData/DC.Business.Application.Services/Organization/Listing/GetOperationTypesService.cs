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
    public class GetOperationTypesService : BusinessService<string, IEnumerable<OperationTypeDto>>, IGetOperationTypesService
    {
        private readonly IListingRepository _listingRepository;
        private IMapper _mapper;

        public GetOperationTypesService(IListingRepository listingRepository, IMapper mapper)
        {
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<OperationResultDto<IEnumerable<OperationTypeDto>>> ExecuteAsync(string inputDto, CancellationToken cancellationToken = default)
        {
            var result = await _listingRepository.GetOperationTypes();

            var mappedResult = _mapper.Map<List<OperationTypeDto>>(result);

            return BuildOperationResultDto(mappedResult);
        }
    }
}

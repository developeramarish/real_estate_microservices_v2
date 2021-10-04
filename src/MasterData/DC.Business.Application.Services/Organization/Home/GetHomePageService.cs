using AutoMapper;
using DC.Business.Application.Contracts.Dtos.Organization.Home;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Home;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Repositories.ElasticSearch;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DC.Business.Application.Services.Organization.Home
{
    public class GetHomePageService : BusinessService<VoidInputDto, HomePageDto>, IGetHomePageService
    {
        private readonly IPropertiesElasticRepository _propertiesElasticRepository;
        private readonly IListingRepository _listingRepository;
        private IMapper _mapper;

        public GetHomePageService(IPropertiesElasticRepository propertiesElasticRepository, 
            IListingRepository listingRepository,
            IMapper mapper)
        {
            _propertiesElasticRepository = propertiesElasticRepository ?? throw new ArgumentNullException(nameof(propertiesElasticRepository));
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override async Task<OperationResultDto<HomePageDto>> ExecuteAsync(VoidInputDto inputDto, CancellationToken cancellationToken = default)
        {
            var result = new HomePageDto();
            var propertyTypes = await _listingRepository.GetPropertyTypes();
            propertyTypes = propertyTypes.ToList();

            var houseType = propertyTypes.FirstOrDefault(x => x.Type == Domain.Enums.PropertyTypeEnum.House);
            result.HotPropertiesToBuy = await GetHotPropertiesToBuyAsync(houseType);

            var roomType = propertyTypes.FirstOrDefault(x => x.Type == Domain.Enums.PropertyTypeEnum.Room);
            result.NewRoomsToRent = await GetNewRoomsToRentAsync(roomType);

            var apartmentType = propertyTypes.FirstOrDefault(x => x.Type == Domain.Enums.PropertyTypeEnum.Apartment);
            result.NewApartmentsToBuy = await GetNewApartmentsToBuyAsync(apartmentType);

            return BuildOperationResultDto(result);
        }

        public async Task<List<PropertyBasicDto>> GetHotPropertiesToBuyAsync(PropertyType type) {
            var result = await _propertiesElasticRepository.GetTop4NewHousesAsync(type.Id);
            List<PropertyBasicDto> list = new List<PropertyBasicDto>();
            if(result != null && result?.Count() > 0)
            {
                foreach (var property in result)
                {
                    var mappedProperty = _mapper.Map<PropertyBasicDto>(property);

                    // TODO check if no photos then skip
                    if (property.Images?.Count > 0)
                    {
                        mappedProperty.MainPhotoUrl = property.Images[0].ImageUrl;
                    }

                    list.Add(mappedProperty);
                }
            }
            return list;
        }
        public async Task<List<PropertyBasicDto>> GetNewRoomsToRentAsync(PropertyType type) {
            var result = await _propertiesElasticRepository.GetTop4NewRoomsAsync(type.Id);
            List<PropertyBasicDto> list = new List<PropertyBasicDto>();
            if (result != null && result?.Count() > 0)
            {
                foreach (var property in result)
                {
                    var mappedProperty = _mapper.Map<PropertyBasicDto>(property);

                    // TODO check if no photos then skip
                    if (property.Images?.Count > 0)
                    {
                        mappedProperty.MainPhotoUrl = property.Images[0].ImageUrl;
                    }

                    list.Add(mappedProperty);
                }
            }
            return list;
        }
        public async Task<List<PropertyBasicDto>> GetNewApartmentsToBuyAsync(PropertyType type) {
            var result = await _propertiesElasticRepository.GetTop4NewApartmentsAsync(type.Id);
            List<PropertyBasicDto> list = new List<PropertyBasicDto>();
            if (result != null && result?.Count() > 0)
            {
                foreach (var property in result)
                {
                    var mappedProperty = _mapper.Map<PropertyBasicDto>(property);

                    // TODO check if no photos then skip
                    if (property.Images?.Count > 0)
                    {
                        mappedProperty.MainPhotoUrl = property.Images[0].ImageUrl;
                    }

                    list.Add(mappedProperty);
                }
            }
            return list;
        }
    }
}

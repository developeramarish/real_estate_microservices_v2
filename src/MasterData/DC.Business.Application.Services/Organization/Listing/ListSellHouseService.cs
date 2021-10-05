using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.RabbitMQ;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Listing;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Enums;
using DC.Business.Domain.Repositories.ElasticSearch;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Errors;

namespace DC.Business.Application.Services.Organization.Listing
{
    public class ListSellHouseService : BusinessService<SellHouseDto, ulong>, IListSellHouseService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertiesElasticRepository _propertiesElasticRepository;
        private IMapper _mapper;
        private readonly IRabbitMQClient _rabbitMqClient;

        public ListSellHouseService(IListingRepository listingRepository, 
            IPropertiesElasticRepository propertiesElasticRepository, IMapper mapper,
            IUserRepository userRepository,
            IRabbitMQClient rabbitMqClient)
        {
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _propertiesElasticRepository = propertiesElasticRepository ?? throw new ArgumentNullException(nameof(propertiesElasticRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _rabbitMqClient = rabbitMqClient ?? throw new ArgumentNullException(nameof(rabbitMqClient));
        }

        protected override async Task<OperationResultDto<ulong>> ExecuteAsync(SellHouseDto sellHouseDto, CancellationToken cancellationToken = default)
        {
            List<ErrorDto> executionErrors = ValidateInput(sellHouseDto);
            if (executionErrors.Any()) return BuildOperationResultDto(executionErrors);

            var user = await _userRepository.GetUserByIdAsync(sellHouseDto.UserId);
            if (user == null) return BuildOperationResultDto(new List<ErrorDto>() { new ErrorDto() { Code = ErrorCodes.ELASTIC_PROPERTY_NOT_FOUND } });

            var property = new Property()
            {
                UserId = sellHouseDto.UserId,
                PropertyTypeId = sellHouseDto.PropertyTypeId,
                OperationTypeId = sellHouseDto.OperationTypeId,
                Price = sellHouseDto.Price,
                NetAream2 = sellHouseDto.NetAream2,
                PriceNetAream2 = sellHouseDto.PriceNetAream2,
                GrossAream2 = sellHouseDto.GrossAream2,
                Typology = sellHouseDto.Typology,
                Floor = sellHouseDto.Floor,
                YearOfConstruction = sellHouseDto.YearOfConstruction,
                NumberOfBathrooms = sellHouseDto.NumberOfBathrooms,
                EnerergyCertificate = sellHouseDto.EnerergyCertificate,
                Country = sellHouseDto.Country,
                City = sellHouseDto.City,
                Address = sellHouseDto.Address,
                Description = sellHouseDto.Description,
                State = PropertyStateEnum.NotApproved,
                //PropertyType = sellHouseDto.ope,
                //OperationType = PropertyStateEnum.NotApproved,
                CreationDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Deleted = false,
                Latitude = sellHouseDto.Latitude,
                Longitude = sellHouseDto.Longitude
            };

            var mySqlId = await _listingRepository.ListProperty(property);
            if(mySqlId <= 0)
                return BuildOperationResultDto(new List<ErrorDto>(){ new ErrorDto(ErrorCodes.ID_NOT_FOUND, nameof(mySqlId))});

            List<Characteristics> characteristics = new List<Characteristics>();
            if(sellHouseDto.Characteristics != null)
            {
                var tasks = new List<Task>();
                foreach(var charact in sellHouseDto.Characteristics)
                {
                    var newChar = new Characteristics();
                    newChar.Create((int)mySqlId, charact.Name, charact.CountNumber, charact.IconName);
                    tasks.Add(_listingRepository.AddCharacteristics(newChar));
                    characteristics.Add(newChar);
                }

                await Task.WhenAll(tasks);
                
            }

            var elasticProperty = new Domain.ElasticEntities.Dto.PropertyInsertDto()
            {
                UserId = sellHouseDto.UserId,
                MySqlId = mySqlId,
                PropertyTypeId = sellHouseDto.PropertyTypeId,
                OperationTypeId = sellHouseDto.OperationTypeId,
                Price = sellHouseDto.Price,
                NetAream2 = sellHouseDto.NetAream2,
                PriceNetAream2 = sellHouseDto.PriceNetAream2,
                GrossAream2 = sellHouseDto.GrossAream2,
                Typology = sellHouseDto.Typology,
                Floor = sellHouseDto.Floor,
                YearOfConstruction = sellHouseDto.YearOfConstruction,
                NumberOfBathrooms = sellHouseDto.NumberOfBathrooms,
                EnerergyCertificate = sellHouseDto.EnerergyCertificate,
                Country = sellHouseDto.Country,
                City = sellHouseDto.City,
                Address = sellHouseDto.Address,
                Description = sellHouseDto.Description,
                Latitude = sellHouseDto.Latitude,
                Longitude = sellHouseDto.Longitude,
                State = PropertyStateEnum.NotApproved,
                CreationDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Characteristics = _mapper.Map<List<Domain.ElasticEntities.Characteristics>>(characteristics)
            };

            await _propertiesElasticRepository.AddPropertyAsync(elasticProperty);

            var payload = JsonSerializer.Serialize(new { Email = user.Email });
            await _rabbitMqClient.PublishMessageAsync( "CreateProperty", payload, "property.created");

            return BuildOperationResultDto(mySqlId);
        }

        private List<ErrorDto> ValidateInput(SellHouseDto sellHouseDt0)
        {
            List<ErrorDto> validationsErros = new List<ErrorDto>();
            if (sellHouseDt0 == null)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.PropertyTypeId == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.OperationTypeId == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.Price == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.NetAream2 == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.Latitude == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.Longitude == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.NumberOfBathrooms == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (string.IsNullOrEmpty(sellHouseDt0.Typology))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (string.IsNullOrEmpty(sellHouseDt0.Description))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (string.IsNullOrEmpty(sellHouseDt0.Address))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (string.IsNullOrEmpty(sellHouseDt0.City))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (string.IsNullOrEmpty(sellHouseDt0.Country))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            if (sellHouseDt0.GrossAream2 == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt0)));

            return validationsErros;
        }
    }
}

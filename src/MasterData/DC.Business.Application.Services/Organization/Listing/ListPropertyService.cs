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
    public class ListPropertyService : BusinessService<SellHouseDto, ulong>, IListPropertyService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertiesElasticRepository _propertiesElasticRepository;
        private IMapper _mapper;
        private readonly IRabbitMQClient _rabbitMqClient;

        public ListPropertyService(IListingRepository listingRepository, 
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

            var property = _mapper.Map<Property>(sellHouseDto);
            property.CreationDate = DateTime.Now;
            property.UpdateDate = DateTime.Now;
            property.Deleted = false;

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

            var elasticProperty = _mapper.Map<Domain.ElasticEntities.Dto.PropertyInsertDto>(sellHouseDto);
            elasticProperty.MySqlId = mySqlId;
            elasticProperty.CreationDate = DateTime.Now;
            elasticProperty.UpdateDate = DateTime.Now;
            elasticProperty.Deleted = false;

            await _propertiesElasticRepository.AddPropertyAsync(elasticProperty);

            var payload = JsonSerializer.Serialize(new { Email = user.Email });
            await _rabbitMqClient.PublishMessageAsync("CreatedProperty", payload, "email_key");

            return BuildOperationResultDto(mySqlId);
        }

        private List<ErrorDto> ValidateInput(SellHouseDto sellHouseDto)
        {
            List<ErrorDto> validationsErros = new List<ErrorDto>();
            if (sellHouseDto == null)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto)));

            if (sellHouseDto.PropertyTypeId == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.PropertyTypeId)));

            if (sellHouseDto.OperationTypeId == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.OperationTypeId)));

            if (sellHouseDto.Price == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Price)));

            if (sellHouseDto.NetAream2 == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.NetAream2)));

            if (sellHouseDto.Latitude == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Latitude)));

            if (sellHouseDto.Longitude == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Longitude)));

            if (sellHouseDto.NumberOfBathrooms == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.NumberOfBathrooms)));

            if (string.IsNullOrEmpty(sellHouseDto.Typology))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Typology)));

            if (string.IsNullOrEmpty(sellHouseDto.Description))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Description)));

            if (string.IsNullOrEmpty(sellHouseDto.Address))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Address)));

            if (string.IsNullOrEmpty(sellHouseDto.City))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.City)));

            if (string.IsNullOrEmpty(sellHouseDto.Country))
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.Country)));

            if (sellHouseDto.GrossAream2 == 0)
                validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDto.GrossAream2)));

            return validationsErros;
        }
    }
}

using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Listing;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Entities.Organization;
using DC.Business.Domain.Enums;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Errors;
using DC.Core.Contracts.Application.Pipeline.Dtos.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DC.Business.Application.Services.Organization.Listing
{
    public class TempListingForUnauthenticatedService : BusinessService<TempPropertyInputDto, ulong>, ITempListingForUnauthenticatedService
    {
        private readonly IListingRepository _listingRepository;
       

        public TempListingForUnauthenticatedService(IListingRepository listingRepository)
        {
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
        }
        protected override async Task<OperationResultDto<ulong>> ExecuteAsync(TempPropertyInputDto inputDto, CancellationToken cancellationToken = default)
        {
            List<ErrorDto> executionErrors = ValidateInput(inputDto.Property);
            if (executionErrors.Any()) return BuildOperationResultDto(executionErrors);

            var property = new Property()
            {
                TempId = inputDto.Id,
                PropertyTypeId = inputDto.Property.PropertyTypeId,
                OperationTypeId = inputDto.Property.OperationTypeId,
                Price = inputDto.Property.Price,
                NetAream2 = inputDto.Property.NetAream2,
                PriceNetAream2 = inputDto.Property.PriceNetAream2,
                GrossAream2 = inputDto.Property.GrossAream2,
                Typology = inputDto.Property.Typology,
                Floor = inputDto.Property.Floor,
                YearOfConstruction = inputDto.Property.YearOfConstruction,
                NumberOfBathrooms = inputDto.Property.NumberOfBathrooms,
                EnerergyCertificate = inputDto.Property.EnerergyCertificate,
                Country = inputDto.Property.Country,
                City = inputDto.Property.City,
                Address = inputDto.Property.Address,
                Description = inputDto.Property.Description,
                State = PropertyStateEnum.NotApproved,
                CreationDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Deleted = false,
                Latitude = inputDto.Property.Latitude,
                Longitude = inputDto.Property.Longitude
            };

            var mySqlId = await _listingRepository.ListTempProperty(property);
            if (mySqlId <= 0)
                return BuildOperationResultDto(new List<ErrorDto>() { new ErrorDto(ErrorCodes.ID_NOT_FOUND, nameof(mySqlId)) });

            List<Characteristics> characteristics = new List<Characteristics>();
            if (inputDto.Property.Characteristics != null)
            {
                var tasks = new List<Task>();
                foreach (var charact in inputDto.Property.Characteristics)
                {
                    var newChar = new Characteristics();
                    newChar.Create((int)mySqlId, charact.Name, charact.CountNumber, charact.IconName);
                    tasks.Add(_listingRepository.AddTempCharacteristics(newChar));
                    characteristics.Add(newChar);
                }

                await Task.WhenAll(tasks);

            }

            return BuildOperationResultDto(mySqlId);
        }

        private List<ErrorDto> ValidateInput(SellHouseDto input)
        {

            List<ErrorDto> validationsErros = new List<ErrorDto>();
            //if (sellHouseDt == null)
            //    validationsErros.Add(new ErrorDto(ErrorCodes.REQUIRED_FILED_IS_EMPTY, nameof(sellHouseDt)));

            return validationsErros;
        }
    }
}

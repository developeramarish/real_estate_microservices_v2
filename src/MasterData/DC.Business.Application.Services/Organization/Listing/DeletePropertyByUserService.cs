using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DC.Business.Application.Contracts.Dtos.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Organization.Listing;
using DC.Business.Application.Contracts.Interfaces.Services;
using DC.Business.Application.Services.Pipeline;
using DC.Business.Domain.Repositories.ElasticSearch;
using DC.Business.Domain.Repositories.Organization;
using DC.Core.Contracts.Application.Pipeline.Dtos;
using DC.Core.Contracts.Application.Pipeline.Dtos.Output;

namespace DC.Business.Application.Services.Organization.Listing
{
    public class DeletePropertyByUserService : BusinessService<PropertyByIdUserIdDto, VoidOutputDto>, IDeletePropertyByUserService
    {
        private readonly IListingRepository _listingRepository;
        private readonly IPropertiesElasticRepository _propertiesElasticRepository;
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;

        public DeletePropertyByUserService(IListingRepository listingRepository, IPropertiesElasticRepository propertiesElasticRepository, IImageService imageService, IImageRepository imageRepository)
        {
            _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
            _propertiesElasticRepository = propertiesElasticRepository ?? throw new ArgumentNullException(nameof(propertiesElasticRepository));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        protected override async Task<OperationResultDto<VoidOutputDto>> ExecuteAsync(PropertyByIdUserIdDto inputDto, CancellationToken cancellationToken = default)
        {
            var propertyToDelete = await _listingRepository.GetPropertyById(inputDto.id);
            if(propertyToDelete == null)
                return BuildOperationResultDto(new VoidOutputDto());

            var images = await _imageRepository.GetPropertyImagesById(inputDto.id);
            if (images != null || images?.Count != 0)
            {
                var imagesUrl = images.Select(x => x.ImageUrl).ToList();
                await _imageService.DeleteAzurePropertyProfileImages(imagesUrl, inputDto.id);
            }

            await _listingRepository.DeletePropertyByUser(inputDto);
            await _propertiesElasticRepository.DeletePropertyByMySqlId(inputDto.id);

            return BuildOperationResultDto(new VoidOutputDto());
        }
    }
}

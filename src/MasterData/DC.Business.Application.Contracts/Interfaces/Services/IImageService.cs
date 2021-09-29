using DC.Business.Application.Contracts.Dtos.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Application.Contracts.Interfaces.Services
{
    public interface IImageService
    {
        Task AddPropertyImagePath(long propertyId, string imageName, string imagePath);
        Task AddPropertyImagePathToElastic(int propertyId, List<PropertyImageDto> propertyImages);
        Task<List<string>> UploadPropertyImagesToBlob(List<IFormFile> photos, long propertyId);
        Task<List<string>> UploadPropertyTempImagesToBlob(List<IFormFile> photos, long propertyId);
        Task DeleteAzurePropertyProfileImages(List<string> imageUrls, long propertyMySqlId);
        Task DeletePropertyProfileImages(List<string> imageUrls, string propertyElasticId, long propertyMySqlId);
        Task<string> UploadUserImageProfileImageToBlob(IFormFile photo, string userEmail);
        Task DeleteUserProfileImage(UserProfileImageDto input);
    }
}

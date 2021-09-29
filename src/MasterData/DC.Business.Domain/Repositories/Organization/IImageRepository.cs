using DC.Business.Domain.Entities.Organization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DC.Business.Domain.Repositories.Organization
{
    public interface IImageRepository
    {
        Task AddPropertyPhoto(long propertyId, string imageName, string imagePath);
        Task AddPropertyTempPhoto(long propertyId, string imageName, string imagePath);
        Task DeletePropertyPhoto(string imagePath, long propertyId);
        Task DeleteBulkPropertyTempPhoto(long propertyId);
        Task<List<Image>> GetPropertyImagesById(long id);
    }
}

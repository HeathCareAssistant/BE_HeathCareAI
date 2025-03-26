using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadDrugImageAsync(string drugId, IFormFile file);
        Task<List<string>> UploadMultipleDrugImagesAsync(string drugId, List<IFormFile> files);
        Task<string> DeleteDrugImageAsync(string drugId);
        Task<string> GetDrugImageUrlAsync(string drugId);
        Task<List<string>> GetAllDrugImagesAsync(string drugId);  
        Task<string> UpdateDrugImageAsync(string drugId, IFormFile file);

        Task<string> UploadDrugPdfAsync(string drugId, IFormFile file);
        Task<string> DeleteDrugPdfAsync(string drugId);
        Task<string> GetDrugPdfUrlAsync(string drugId);
        Task<string> UpdateDrugPdfAsync(string drugId, IFormFile file);

        Task<string> UploadUserImageAsync(string userId, IFormFile file);
        Task<string> DeleteUserImageAsync(string userId);
        Task<string> GetUserImageUrlAsync(string userId);

    }
}

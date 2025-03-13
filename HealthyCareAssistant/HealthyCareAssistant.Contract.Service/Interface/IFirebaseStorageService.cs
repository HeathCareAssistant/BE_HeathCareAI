using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadDrugImageAsync(string drugId, IFormFile file);
        Task<string> GetDrugImageUrlAsync(string drugId);
        Task<string> DeleteDrugImageAsync(string drugId);
        Task UploadMultipleFilesAsync(string drugId, List<IFormFile> files);
        Task<string> UploadDrugPdfAsync(string drugId, IFormFile file);
        Task<string> GetDrugPdfUrlAsync(string drugId);
        Task<string> DeleteDrugPdfAsync(string drugId);
        Task<string> UploadUserImageAsync(string userId, IFormFile file);

        Task<string> GetUserImageUrlAsync(string userId); 
        Task<string> DeleteUserImageAsync(string userId);



    }
}

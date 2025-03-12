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
        Task<string> GetDrugImageUrlAsync(string drugId, string fileName);
        Task<bool> DeleteDrugImageAsync(string drugId, string fileName);
        Task UploadMultipleFilesAsync(string drugId, List<IFormFile> files);
        Task<string> UploadDrugPdfAsync(string drugId, IFormFile file);
        Task<string> GetDrugPdfUrlAsync(string drugId, string fileName);
        Task<bool> DeleteDrugPdfAsync(string drugId, string fileName);
        Task<string> UploadUserImageAsync(string userId, IFormFile file);
        Task<string> GetUserImgUrlAsync(string userId, string fileName);
        Task<bool> DeleteUserImgAsync(string userid, string fileName);
    }
}

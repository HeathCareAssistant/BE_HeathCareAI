using Firebase.Auth;
using Firebase.Storage;
using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// ✅ Tránh xung đột tên `User`
using AppUser = HealthyCareAssistant.Contact.Repo.Entity.User;

namespace HealthyCareAssistant.Service.Service.firebase
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly IFirebaseSetting _firebaseSettings;
        private readonly IUnitOfWork _unitOfWork;
       

        public FirebaseStorageService(IOptions<IFirebaseSetting> options, IUnitOfWork unitOfWork)
        {
            _firebaseSettings = options.Value;
            _unitOfWork = unitOfWork;
        }

        private async Task<FirebaseAuthLink> AuthenticateFirebaseAsync()
        {
            if (string.IsNullOrWhiteSpace(_firebaseSettings.ApiKey) ||
                string.IsNullOrWhiteSpace(_firebaseSettings.AuthEmail) ||
                string.IsNullOrWhiteSpace(_firebaseSettings.AuthPassword))
            {
                throw new InvalidOperationException("Firebase authentication credentials are missing.");
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));
            return await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.AuthEmail, _firebaseSettings.AuthPassword);
        }

        // ===================== UPLOAD DRUG IMAGE =====================
        public async Task<string> UploadDrugImageAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            var uploadedUrl = await UploadToFirebase($"drug-image/{drugId}/{file.FileName}", file);
            await UpdateDrugImageInDb(drugId, uploadedUrl);
            return uploadedUrl;
        }

        public async Task<List<string>> UploadMultipleDrugImagesAsync(string drugId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("No files provided");

            List<string> uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;
                string uploadedUrl = await UploadToFirebase($"drug-image/{drugId}/{file.FileName}", file);
                uploadedUrls.Add(uploadedUrl);
            }

            await UpdateDrugImageInDb(drugId, string.Join(";", uploadedUrls));
            return uploadedUrls;
        }

        private async Task UpdateDrugImageInDb(string drugId, string imageUrl)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug != null)
            {
                drug.Images = imageUrl;
                await drugRepo.UpdateAsync(drug);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<string> GetDrugImageUrlAsync(string drugId)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            return drug?.Images ?? $"DrugID {drugId} không có ảnh.";
        }

        public async Task<string> DeleteDrugImageAsync(string drugId)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug == null || string.IsNullOrWhiteSpace(drug.Images))
                return $"DrugID {drugId} không có ảnh để xóa.";

            await DeleteFromFirebase(drug.Images);
            drug.Images = null;
            await drugRepo.UpdateAsync(drug);
            await _unitOfWork.SaveAsync();

            return $"Ảnh của DrugID {drugId} đã được xóa thành công.";
        }
        public async Task<List<string>> GetAllDrugImagesAsync(string drugId)
        {
            string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=drug-image/{drugId}/";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return new List<string>(); // Không có ảnh
                }

                string json = await response.Content.ReadAsStringAsync();
                var fileList = new List<string>();

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            string filePath = item.GetProperty("name").GetString();
                            string imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";
                            fileList.Add(imageUrl);
                        }
                    }
                }
                return fileList;
            }
        }
        public async Task<string> UpdateDrugImageAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            var drugRepo = _unitOfWork.GetRepository<Drug>(); // 🔹 Lấy repository từ UnitOfWork
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug == null)
                return $"DrugID {drugId} không tồn tại.";

            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrWhiteSpace(drug.Images))
                await DeleteFromFirebase(drug.Images);

            // Upload ảnh mới
            var uploadedUrl = await UploadToFirebase($"drug-image/{drugId}/{file.FileName}", file);

            // Cập nhật DB
            drug.Images = uploadedUrl;
            await drugRepo.UpdateAsync(drug);
            await _unitOfWork.SaveAsync();

            return uploadedUrl;
        }


        // ===================== UPLOAD DRUG PDF =====================
        public async Task<string> UploadDrugPdfAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            var uploadedUrl = await UploadToFirebase($"drug-pdf/{drugId}/{file.FileName}", file);
            await UpdateDrugPdfInDb(drugId, uploadedUrl);
            return uploadedUrl;
        }

        private async Task UpdateDrugPdfInDb(string drugId, string pdfUrl)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug != null)
            {
                drug.FileName = pdfUrl;
                await drugRepo.UpdateAsync(drug);
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task<string> UpdateDrugPdfAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            var drugRepo = _unitOfWork.GetRepository<Drug>(); // Lấy repository từ UnitOfWork
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug == null)
                return $"DrugID {drugId} không tồn tại.";

            // Xóa PDF cũ nếu có
            if (!string.IsNullOrWhiteSpace(drug.FileName))
                await DeleteFromFirebase(drug.FileName);

            // Upload PDF mới
            var uploadedUrl = await UploadToFirebase($"drug-pdf/{drugId}/{file.FileName}", file);

            // Cập nhật DB
            drug.FileName = uploadedUrl;
            await drugRepo.UpdateAsync(drug);
            await _unitOfWork.SaveAsync();

            return uploadedUrl;
        }

        public async Task<string> GetDrugPdfUrlAsync(string drugId)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            return drug?.FileName ?? $"DrugID {drugId} không có PDF.";
        }

        public async Task<string> DeleteDrugPdfAsync(string drugId)
        {
            var drugRepo = _unitOfWork.GetRepository<Drug>();
            var drug = await drugRepo.GetByIdAsync(drugId);
            if (drug == null || string.IsNullOrWhiteSpace(drug.FileName))
                return $"DrugID {drugId} không có PDF để xóa.";

            await DeleteFromFirebase(drug.FileName);
            drug.FileName = null;
            await drugRepo.UpdateAsync(drug);
            await _unitOfWork.SaveAsync();

            return $"PDF của DrugID {drugId} đã được xóa thành công.";
        }

        // ===================== UPLOAD USER IMAGE =====================
        public async Task<string> UploadUserImageAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            var uploadedUrl = await UploadToFirebase($"user-image/{userId}/{file.FileName}", file);
            await UpdateUserImageInDb(userId, uploadedUrl);
            return uploadedUrl;
        }
        public async Task<string> DeleteUserImageAsync(string userId)
        {
            var userRepo = _unitOfWork.GetRepository<AppUser>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null || string.IsNullOrWhiteSpace(user.Image))
                return $"UserID {userId} không có ảnh để xóa.";

            await DeleteFromFirebase(user.Image);
            user.Image = null;
            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return $"Ảnh của UserID {userId} đã được xóa thành công.";
        }
        private async Task DeleteFromFirebase(string fileUrl)
        {
            try
            {
                var account = await AuthenticateFirebaseAsync();
                var storage = new FirebaseStorage(_firebaseSettings.Bucket, new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                    ThrowOnCancel = true
                });

                // Trích xuất đường dẫn file từ URL Firebase
                string filePath = fileUrl.Split("/o/")[1].Split("?alt=")[0];

                // Xóa file trên Firebase
                await storage.Child(Uri.UnescapeDataString(filePath)).DeleteAsync();

                Console.WriteLine($"[DEBUG] File deleted from Firebase: {fileUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete file from Firebase: {ex.Message}");
                throw;
            }
        }

        private async Task UpdateUserImageInDb(string userId, string imageUrl)
        {
            var userRepo = _unitOfWork.GetRepository<AppUser>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user != null)
            {
                user.Image = imageUrl;
                await userRepo.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<string> GetUserImageUrlAsync(string userId)
        {
            var userRepo = _unitOfWork.GetRepository<AppUser>();
            var user = await userRepo.GetByIdAsync(userId);
            return user?.Image ?? $"UserID {userId} không có ảnh.";
        }

        private async Task<string> UploadToFirebase(string filePath, IFormFile file)
        {
            var account = await AuthenticateFirebaseAsync();
            var storage = new FirebaseStorage(_firebaseSettings.Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                ThrowOnCancel = true
            });

            using (var stream = file.OpenReadStream())
            {
                return await storage.Child(filePath).PutAsync(stream);
            }
        }
    }
}

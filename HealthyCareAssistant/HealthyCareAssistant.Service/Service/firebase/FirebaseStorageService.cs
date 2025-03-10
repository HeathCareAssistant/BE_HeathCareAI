using Firebase.Auth;
using Firebase.Storage;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service.firebase
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly FirebaseSettings _firebaseSettings;

        public FirebaseStorageService(IOptions<FirebaseSettings> options)
        {
            _firebaseSettings = options.Value;
        }

        /// <summary>
        /// Xác thực Firebase và lấy token
        /// </summary>
        private async Task<FirebaseAuthLink> AuthenticateFirebaseAsync()
        {
            Console.WriteLine($"[DEBUG] Firebase API Key: {_firebaseSettings.ApiKey}");
            Console.WriteLine($"[DEBUG] Firebase Auth Email: {_firebaseSettings.AuthEmail}");

            if (string.IsNullOrWhiteSpace(_firebaseSettings.ApiKey) ||
                string.IsNullOrWhiteSpace(_firebaseSettings.AuthEmail) ||
                string.IsNullOrWhiteSpace(_firebaseSettings.AuthPassword))
            {
                throw new InvalidOperationException("Firebase authentication credentials are missing or invalid.");
            }

            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));
                var authResult = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.AuthEmail, _firebaseSettings.AuthPassword);

                Console.WriteLine("[DEBUG] Successfully authenticated with Firebase.");
                return authResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Firebase Authentication Failed: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Tải ảnh thuốc lên Firebase Storage
        /// </summary>
        public async Task<string> UploadDrugImageAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            Console.WriteLine($"[DEBUG] Uploading image for DrugID: {drugId}, File: {file.FileName}");

            try
            {
                var account = await AuthenticateFirebaseAsync();
                var storage = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                        ThrowOnCancel = true
                    });

                string fileName = $"drug-image/{drugId}/{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var uploadTask = storage.Child(fileName).PutAsync(stream);
                    string uploadedUrl = await uploadTask;

                    Console.WriteLine($"[DEBUG] Upload successful: {uploadedUrl}");
                    return uploadedUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Upload failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy URL truy cập ảnh thuốc từ Firebase Storage
        /// </summary>
        public async Task<string> GetDrugImageUrlAsync(string drugId, string fileName)
        {
            string filePath = $"drug-image/{drugId}/{fileName}";
            string imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";

            Console.WriteLine($"[DEBUG] Fetching image URL: {imageUrl}");
            return imageUrl;
        }

        /// <summary>
        /// Xóa ảnh thuốc trong Firebase Storage
        /// </summary>
        public async Task<bool> DeleteDrugImageAsync(string drugId, string fileName)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Deleting image: drug-image/{drugId}/{fileName}");

                var account = await AuthenticateFirebaseAsync();
                var storage = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                        ThrowOnCancel = true
                    });

                await storage.Child($"drug-image/{drugId}/{fileName}").DeleteAsync();

                Console.WriteLine("[DEBUG] Deletion successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Deletion failed: {ex.Message}");
                return false;
            }
        }
        public async Task UploadMultipleFilesAsync(string drugId, List<IFormFile> files)
        {
            var account = await AuthenticateFirebaseAsync();
            var storage = new FirebaseStorage(
                _firebaseSettings.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                    ThrowOnCancel = true
                });

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                string fileName = $"drug-image/{drugId}/{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var uploadTask = storage.Child(fileName).PutAsync(stream);
                    await uploadTask; // Đợi upload hoàn tất
                }
            }

            Console.WriteLine($"Uploaded {files.Count} files for DrugID: {drugId}");
        }
        /// <summary>
        /// Upload PDF lên Firebase Storage
        /// </summary>
        public async Task<string> UploadDrugPdfAsync(string drugId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            Console.WriteLine($"[DEBUG] Uploading PDF for DrugID: {drugId}, File: {file.FileName}");

            try
            {
                var storage = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    });

                string fileName = $"drug-pdf/{drugId}/{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var uploadTask = storage.Child(fileName).PutAsync(stream);
                    string uploadedUrl = await uploadTask;
                    Console.WriteLine($"[DEBUG] PDF Upload successful: {uploadedUrl}");
                    return uploadedUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] PDF Upload failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy URL truy cập PDF từ Firebase
        /// </summary>
        public async Task<string> GetDrugPdfUrlAsync(string drugId, string fileName)
        {
            string filePath = $"drug-pdf/{drugId}/{fileName}";
            string pdfUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";

            Console.WriteLine($"[DEBUG] Fetching PDF URL: {pdfUrl}");
            return pdfUrl;
        }

        /// <summary>
        /// Xóa PDF từ Firebase Storage
        /// </summary>
        public async Task<bool> DeleteDrugPdfAsync(string drugId, string fileName)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Deleting PDF: drug-pdf/{drugId}/{fileName}");

                var storage = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    });

                await storage.Child($"drug-pdf/{drugId}/{fileName}").DeleteAsync();

                Console.WriteLine("[DEBUG] PDF Deletion successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] PDF Deletion failed: {ex.Message}");
                return false;
            }
        }
    }
}


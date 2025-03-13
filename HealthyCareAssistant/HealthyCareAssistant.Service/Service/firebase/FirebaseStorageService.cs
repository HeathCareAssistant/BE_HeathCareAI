using Firebase.Auth;
using Firebase.Storage;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
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
        public async Task<string> GetDrugImageUrlAsync(string drugId)
        {
            string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=drug-image/{drugId}/";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return $"DrugID {drugId} không có ảnh.";
                }

                string json = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                    {
                        string filePath = items[0].GetProperty("name").GetString();
                        string imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";
                        return imageUrl;
                    }
                }
            }

            return $"DrugID {drugId} không có ảnh.";
        }


        /// <summary>
        /// Xóa ảnh thuốc trong Firebase Storage
        /// </summary>
        public async Task<string> DeleteDrugImageAsync(string drugId)
        {
            try
            {
                string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=drug-image/{drugId}/";

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"DrugID {drugId} không có ảnh để xóa.";
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                        {
                            string filePath = items[0].GetProperty("name").GetString();

                            var account = await AuthenticateFirebaseAsync();
                            var storage = new FirebaseStorage(
                                _firebaseSettings.Bucket,
                                new FirebaseStorageOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                                    ThrowOnCancel = true
                                });

                            await storage.Child(filePath).DeleteAsync();
                            return $"Ảnh của DrugID {drugId} đã được xóa thành công.";
                        }
                    }
                }

                return $"DrugID {drugId} không có ảnh để xóa.";
            }
            catch (Exception ex)
            {
                return $"Xóa ảnh thất bại: {ex.Message}";
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
        public async Task<string> GetDrugPdfUrlAsync(string drugId)
        {
            string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=drug-pdf/{drugId}/";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return $"DrugID {drugId} không có PDF.";
                }

                string json = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                    {
                        string filePath = items[0].GetProperty("name").GetString();
                        string pdfUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";
                        return pdfUrl;
                    }
                }
            }

            return $"DrugID {drugId} không có PDF.";
        }


        /// <summary>
        /// Xóa PDF từ Firebase Storage
        /// </summary>
        public async Task<string> DeleteDrugPdfAsync(string drugId)
        {
            try
            {
                string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=drug-pdf/{drugId}/";

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"DrugID {drugId} không có PDF để xóa.";
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                        {
                            string filePath = items[0].GetProperty("name").GetString();

                            var account = await AuthenticateFirebaseAsync();
                            var storage = new FirebaseStorage(
                                _firebaseSettings.Bucket,
                                new FirebaseStorageOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                                    ThrowOnCancel = true
                                });

                            await storage.Child(filePath).DeleteAsync();
                            return $"PDF của DrugID {drugId} đã được xóa thành công.";
                        }
                    }
                }

                return $"DrugID {drugId} không có PDF để xóa.";
            }
            catch (Exception ex)
            {
                return $"Xóa PDF thất bại: {ex.Message}";
            }
        }

        /// <summary>
        /// Tải ảnh user lên Firebase Storage
        /// </summary>
        public async Task<string> UploadUserImageAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("The file is empty");

            Console.WriteLine($"[DEBUG] Uploading image for USERID: {userId}, File: {file.FileName}");

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

                string fileName = $"user-image/{userId}/{file.FileName}";

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
        /// Lấy URL truy cập image user từ Firebase
        /// </summary>
        public async Task<string> GetUserImageUrlAsync(string userId)
        {
            string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=user-image/{userId}/";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return $"UserID {userId} không có ảnh.";
                }

                string json = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                    {
                        string filePath = items[0].GetProperty("name").GetString();
                        string imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o/{Uri.EscapeDataString(filePath)}?alt=media";
                        return imageUrl;
                    }
                }
            }

            return $"UserID {userId} không có ảnh.";
        }

        /// <summary>
        /// Xóa img user từ Firebase Storage
        /// </summary>
        public async Task<string> DeleteUserImageAsync(string userId)
        {
            try
            {
                string apiUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.Bucket}/o?prefix=user-image/{userId}/";

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        return $"UserID {userId} không có ảnh để xóa.";
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("items", out JsonElement items) && items.GetArrayLength() > 0)
                        {
                            string filePath = items[0].GetProperty("name").GetString();

                            var account = await AuthenticateFirebaseAsync();
                            var storage = new FirebaseStorage(
                                _firebaseSettings.Bucket,
                                new FirebaseStorageOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                                    ThrowOnCancel = true
                                });

                            await storage.Child(filePath).DeleteAsync();
                            return $"Ảnh của UserID {userId} đã được xóa thành công.";
                        }
                    }
                }

                return $"UserID {userId} không có ảnh để xóa.";
            }
            catch (Exception ex)
            {
                return $"Xóa ảnh thất bại: {ex.Message}";
            }
        }

    }
}


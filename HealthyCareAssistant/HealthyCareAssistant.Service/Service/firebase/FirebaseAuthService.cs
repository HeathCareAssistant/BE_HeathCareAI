using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service.firebase
{
    public class FirebaseAuthService
    {
        private readonly FirebaseApp _firebaseApp;

        public FirebaseAuthService(IConfiguration configuration)
        {
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), serviceAccountPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Không tìm thấy file Firebase JSON: {fullPath}");
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                _firebaseApp = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(fullPath)
                });
            }
            else
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
            }
        }

        /// <summary>
        /// Xác thực ID Token với Firebase
        /// </summary>
        public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            }
            catch (FirebaseAuthException ex)
            {
                throw new UnauthorizedAccessException("Token không hợp lệ", ex);
            }
        }
    }
}

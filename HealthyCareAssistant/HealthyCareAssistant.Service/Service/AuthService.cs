using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HealthyCareAssistant.Service.Service
{
    public class AuthService
    {
        private readonly string _firebaseApiKey;
        private readonly HttpClient _httpClient;

        public AuthService(IConfiguration configuration)
        {
            _firebaseApiKey = configuration["Firebase:ApiKey"];
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Đăng nhập Firebase bằng Email & Password để lấy ID Token
        /// </summary>
        public async Task<string> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(_firebaseApiKey))
            {
                throw new InvalidOperationException("Firebase API Key không được để trống.");
            }

            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException($"Lỗi đăng nhập Firebase: {errorResponse}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var firebaseResponse = JsonConvert.DeserializeObject<FirebaseAuthResponse>(responseBody);

            return firebaseResponse.IdToken;
        }
    }

    public class FirebaseAuthResponse
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }
    }
}

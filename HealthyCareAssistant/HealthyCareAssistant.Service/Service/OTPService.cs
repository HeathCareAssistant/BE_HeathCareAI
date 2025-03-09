using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using HealthyCareAssistant.Contract.Service.Interface;

public class OTPService : IOTPService
{
    private readonly IMemoryCache _cache;

    public OTPService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GenerateOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteArray = new byte[4];
        rng.GetBytes(byteArray);
        var otp = BitConverter.ToUInt32(byteArray, 0) % 1000000;
        return otp.ToString("D6");
    }

    public async Task<bool> StoreOtpAsync(string email, string otp)
    {
        var cacheKey = $"OTP_{email}";
        _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
        return await Task.FromResult(true);
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp)
    {
        var cacheKey = $"OTP_{email}";
        if (_cache.TryGetValue(cacheKey, out string storedOtp))
        {
            return await Task.FromResult(storedOtp == otp);
        }
        return await Task.FromResult(false);
    }
}

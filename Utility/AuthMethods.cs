using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend.Utility;

public class AuthMethods{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt){
        using(var hmac = new HMACSHA512()){
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt){
        using(var hmac = new HMACSHA512(passwordSalt)){
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    public static string CreateAuthToken(User user, IConfiguration configuration){
        List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Email, user.Email),
        };
        string? secretKey = configuration.GetSection("AppSettings:SecretKey").Value;
        if (string.IsNullOrEmpty(secretKey)) throw new Exception("failed to load jwt hash secret");
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokens = new JwtSecurityToken(
            claims: claims, 
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokens);
    }

    public static string GenerateAffiliateLink(User user){
        string uniqueToken = Guid.NewGuid().ToString("N");
        return $"api/auth/register?aLink={user.UserId}-{uniqueToken}";
    }

    public static int GetUserIdFromAffiliateLink(string affiliateLink){
        string[] parts = affiliateLink.Split('?')[1].Split('=')[1].Split('-');
        return int.Parse(parts[0]);
    }
}
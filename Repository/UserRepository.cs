using backend.Data;
using backend.Models;
using backend.Utility;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public interface IUserRepository{
    public Task<string> CreateAffiliateLink(string userId);
    public Task<string> AuthenticateUser(UserDTO userDTO);
    public Task<User> CreateNewUser(UserDTO userDTO);
    public Task<User> CreateNewUser(UserDTO userDTO, string? affiliateLink);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext dbContext;
    private readonly IConfiguration _configuration;
    
    public UserRepository(AppDbContext appDbContext, IConfiguration configuration){
        dbContext = appDbContext;
        _configuration = configuration;
    }

    public async Task<string> AuthenticateUser(UserDTO userDTO)
    {
        var dbUser = await dbContext.Users.Where(u=> u.Email == userDTO.Email).FirstOrDefaultAsync();
        if (dbUser == null) throw new Exception("please credentials and try again");
        if (!AuthMethods.VerifyPasswordHash(userDTO.Password, dbUser.PasswordHash, dbUser.PasswordSalt))
            throw new Exception("please check credential and try again");
        try{
            var token = AuthMethods.CreateAuthToken(dbUser, _configuration);
            return token;
        }catch(Exception ex){
            throw ex;
        }
    }

    public async Task<string> CreateAffiliateLink(string email)
    {
        var dbUser = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        if (dbUser == null) throw new Exception("user not found");
        if (!string.IsNullOrEmpty(dbUser.AffiliateLink)) return dbUser.AffiliateLink;
        string affiliateLink = AuthMethods.GenerateAffiliateLink(dbUser);
        dbUser.AffiliateLink = affiliateLink;
        await dbContext.SaveChangesAsync();
        return affiliateLink;
    }

    public async Task<User> CreateNewUser(UserDTO userDTO)
    {
        var dbUser = await dbContext.Users.Where(u=> u.Email == userDTO.Email).FirstOrDefaultAsync();
        if (dbUser != null) throw new Exception("account already exist");
        byte[] passwordHash, passwordSalt;
        AuthMethods.CreatePasswordHash(userDTO.Password, out passwordHash, out passwordSalt);
        var createdUser = new User{Email = userDTO.Email, PasswordSalt = passwordSalt, PasswordHash = passwordHash};
        await dbContext.Users.AddAsync(createdUser);
        await dbContext.SaveChangesAsync();
        return createdUser;
    }

    public async Task<User> CreateNewUser(UserDTO userDTO, string? affiliateLink)
    {
        var child = await CreateNewUser(userDTO);
        if (!string.IsNullOrEmpty(affiliateLink)){
            int parentId = AuthMethods.GetUserIdFromAffiliateLink(affiliateLink);
            var newChild = new Child{ User = child, UserId = child.UserId};
            User? parent = await dbContext.Users.Where(u => u.UserId == parentId).FirstOrDefaultAsync();
            if (parent == null) throw new Exception("parent not found");
            parent.AddChild(newChild);
            await dbContext.SaveChangesAsync();
        }
        return child;
    }
}
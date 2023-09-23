using System.Security.Claims;
using backend.Models;
using backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepo;

    public AuthController(ILogger<AuthController> logger, IUserRepository userRepository){
        _logger = logger;
        _userRepo = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO, [FromQuery] string? aLink){
        try{
            var res = await _userRepo.CreateNewUser(userDTO, aLink);
            return Ok("user created");
        }catch(Exception ex){
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserDTO userDTO){
        try{
            var authToken = await _userRepo.AuthenticateUser(userDTO);
            return Ok(authToken);
        }catch(Exception ex){
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("register/link")]
    public async Task<IActionResult> CreateAffiliateLink(){
        try{
            string? userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            if (string.IsNullOrEmpty(userEmail)) throw new Exception("error, please ensure you authenticated");
            string affiliateLink = await _userRepo.CreateAffiliateLink(userEmail);
            return Ok(affiliateLink);
        }catch(Exception ex){
            return BadRequest(ex.Message);
        }
    }
}
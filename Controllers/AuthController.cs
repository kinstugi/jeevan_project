using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase{
    private readonly ILogger _logger;

    public AuthController(ILogger logger){
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO){
        await Task.Delay(1);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserDTO userDTO){
        await Task.Delay(1);
        return Ok();
    }
}
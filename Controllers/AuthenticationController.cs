using autenticacion.Dtos;
using autenticacion.Models;
using autenticacion.Services;
using autenticacion.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : Controller
{
    private readonly AuthenticationService _service;
    private readonly IHubContext<WorkerHub> _hubContext;

    public AuthenticationController(AuthenticationService service, IHubContext<WorkerHub> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticationDTO loginDto)
    {
        var verify = await _service.VerifyUser(loginDto);

        if (verify == false) {
            return Unauthorized(new ApiResponse<ResponseAuthenticationDTO>("Error", null, "Incorrect username or password"));
        }

        var result = await _service.LoginAsync(loginDto.UserName);

        return Ok(new ApiResponse<ResponseAuthenticationDTO>("success", result, "Login successful"));
    }

    [Authorize]
    [HttpGet("Users")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _service.GetAllAsync();
        return Ok(new ApiResponse<List<UserDTO>>("success", result, "List of users"));
    }

    [Authorize]
    [HttpGet("UsersByCodice/{codice}")]
    public async Task<IActionResult> GetOrderByCode(string codice)
    {

        var result = await _service.GetByCodeAsync(codice);

        if (result == null)
        {
            return Ok(new ApiResponse<UserDTO>("empty", result, "User not found"));
        }

        return Ok(new ApiResponse<UserDTO>("success", result, "User found"));
    }

    [Authorize]
    [HttpPost("Users/add")]
    public async Task<IActionResult> Create([FromBody] User model)
    {
        if (model == null)
        {
            return BadRequest(new ApiResponse<string>("Error", null, "Invalid user data"));
        }

        model.UserCode = await _service.GenerateNextUserCodeAsync();

        var created = await _service.CreateAsync(model);

        if (created == null)
        {
            return StatusCode(500, new ApiResponse<string>("Error", null, "User was created but could not be retrieved"));
        }

        return CreatedAtAction(nameof(Create),
            new { codice = created.UserCode },
            new ApiResponse<UserDTO>("success", created, "User created successfully"));
    }
}


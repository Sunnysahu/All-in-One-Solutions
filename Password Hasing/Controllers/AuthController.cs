using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Password_Hasing.Data;
using Password_Hasing.Dto;
using Password_Hasing.Models;
using Password_Hasing.Service;
using Password_Hasing.Utils.ApiError;
using System.Net;

namespace Password_Hasing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterDto>> Register(RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);

                return result == null ? NotFound(new Api<string>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "User Registration Failed, Already Exist"
                }) : Ok(new RegisterDto
                {
                    Email = result.Email,
                    Password = result.Password,
                    hashedPassword = result.hashedPassword
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("login")]
        public async Task<ActionResult<LoginDto>> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);

                return result.ErrorMessage != "" ? NotFound(new Api<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.ErrorMessage,
                }) : Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

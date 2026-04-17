using JWT.Data;
using JWT.DTO;
using JWT.Models;
using JWT.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Controllers
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


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequestDto loginRequestDto)
        {
            var user = await _authService.Login(loginRequestDto);

            if (user == null) 
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ErrorMessage = "Invalid username or password",
                    Message = "Not A Valid User"
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login Successful",
                User = user
            });
            
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDto refreshRequestDto)
        {
            var refreshToken = await _authService.Refresh(refreshRequestDto.RefreshToken);

            if(refreshToken == null)
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    ErrorMessage = "Unable to Refresh the Token",
                    Message = "Invalid Refresh Token"
                });
            }

            return Ok(refreshToken);
        }


        [HttpGet("publicwithdifferentlibraryproject")]
        public UserDto publicwithdifferentlibraryproject()
        {

            return new UserDto // Getting imported deom different library file, UsrDto.cs
            {
                Name = "Sunny",
            };
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public endpoint");
        }

        [Authorize]
        [HttpGet("private")]
        public IActionResult Private()
        {
            return Ok("Private endpoint");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            var tok = User.Claims.Select(x => new
            {
                x.Type,
                x.Value,
                x.Issuer,
                x.OriginalIssuer,
                x.ValueType
            });

            if (!User.IsInRole("Admin"))
            {
                return Unauthorized("No Premission"); 
            }

            return Ok(tok);
        }

        [Authorize(Roles ="User")]
        [HttpGet("user")]
        public IActionResult UserRoute()
        {
            var tok = User.Claims.Select(x => new
            {
                x.Type,
                x.Value,
                x.Issuer,
                x.OriginalIssuer,
                x.ValueType
            });


            if (User.IsInRole("Admin"))
            {
                return Unauthorized("No Premission");
            }

            return Ok(tok);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("manager")]
        public IActionResult Manager()
        {
            return Ok("Manager only endpoint");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("AdminUser")]
        public IActionResult AdminUser()
        {
            return Ok("Admin & User only endpoint");
        }

        [Authorize]
        [HttpGet("forbidadmin")]
        public IActionResult ForbidAdmin()
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            return Ok("Admin only");
        }
    }
}

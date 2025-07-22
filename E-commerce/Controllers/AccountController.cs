using E_commerce.DTO_s;
using E_commerce.Model;
using E_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly TokenDbService _tokenDbService;

        public AccountController(UserManager<ApplicationUser> userManager, TokenService tokenService, TokenDbService tokenDbService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _tokenDbService = tokenDbService;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
            if (user == null)
            {
                return Unauthorized(new AuthenticatedResponse
                {
                    Message = "Invalid credentials",
                    Token = null
                });
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return Unauthorized(new AuthenticatedResponse
                {
                    Message = "Invalid Password",
                    Token = null
                });
            }
            var access = await _tokenService.GenerateToken(user.Id, user.Email);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenDbService.AddRefreshTokenAsync(refreshToken, user.Id);

            Response.Cookies.Append("token", access, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15),
                Path = "/"
            });
            Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            });

            return Ok(new AuthenticatedResponse
            {
                Message = "Login successful.",
                Token = access,
                RefreshToken = refreshToken.Token
            });
        }

        [HttpPost("SignUp")]
        public async Task<SignupResponseDTO> SignUp([FromBody] SignupRequestDTO request)
        {
            if (request != null)
            {
                var user = new ApplicationUser
                {
                    UserName = request.Name,
                    Email = request.Email,
                    NormalizedUserName = request.Name.ToUpper(),
                    NormalizedEmail = request.Email.ToUpper(),

                };

                var result = await _userManager.CreateAsync(user, request.password);
                if (result.Succeeded)
                {
                    return new SignupResponseDTO
                    {
                        Token = await _tokenService.GenerateToken(user.Id, user.Email),
                        Message = "User created successfully.",
                        Status = StatusCode(201).ToString()
                    };
                }
                else
                {
                    return new SignupResponseDTO
                    {
                        Token = null,
                        Message = result.Errors.FirstOrDefault()?.Description,
                        Status = StatusCode(201).ToString()
                    };
                }
            }
            else
            {
                return new SignupResponseDTO
                {
                    Message = "Invalid request data."
                };
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {

            var token = Request.Cookies["refreshToken"];
            if (token == null || string.IsNullOrEmpty(token))
                return BadRequest("Refresh token is missing.");

            var user = await _tokenDbService.GetRefreshTokenAsync(token);
            if (user == null)
                return Unauthorized("Invalid Refresh Token");

            var oldrefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == token);
            if (oldrefreshToken == null || !oldrefreshToken.IsActive)
                return Unauthorized("Token expired or revoked");

            oldrefreshToken.Revoked = DateTime.UtcNow;
            var newAccessToken = await _tokenService.GenerateToken(user.Id, user.Email);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _tokenDbService.UpdateRefreshTokenAsync(oldrefreshToken);
            await _tokenDbService.AddRefreshTokenAsync(newRefreshToken, user.Id);

            Response.Cookies.Append("token", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(7),
                Path = "/"
            });

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                Path = "/"
            });
            return Ok(new
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }


        //[HttpPost("revoke-token")]
        //public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        //{
        //    var result = await _tokenDbService.RevokeRefreshTokenAsync(request.RefreshToken);

        //    if (!result)
        //        return BadRequest(new { message = "Invalid refresh token." });

        //    return Ok(new { message = "Token revoked successfully." });
        //}

        [Authorize]
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()

        {
            return Ok(new { authenticated = true });
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> Logout()
        {

            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("No refresh token found");
            }

            var revoked = _tokenDbService.RevokeRefreshTokenAsync(refreshToken);
            if (revoked.Result==true)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                };
                Response.Cookies.Delete("token",cookieOptions);
                Response.Cookies.Delete("refreshToken",cookieOptions);
                return Ok(new { message = "Logged out successfully" });
            }
            else{
                return BadRequest(new { message = "Failed to revoke refresh token" });
            }
            
        }
    }
}


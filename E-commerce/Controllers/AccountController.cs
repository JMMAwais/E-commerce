using E_commerce.DTO_s;
using E_commerce.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<IdentityUser> userManager,TokenService tokenService)
        {
                _userManager = userManager;
                _tokenService = tokenService;
        }


        [HttpPost("Login")]
        public async Task<AuthenticatedResponse> Login([FromBody] LoginRequestDTO request)
            {
            var user=await _userManager.FindByEmailAsync(request.Username);
            if (user == null)
            { 
                return new AuthenticatedResponse
                {
                    Token=null,
                    Message = "Unauthorized: user not found"
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if(!result)
            {
                return new AuthenticatedResponse
                {
                    Token = null,
                    Message = "Invalid Password"
                };
            }
           var authenticate=await _tokenService.GenerateToken(user.Id, user.Email);
            return new AuthenticatedResponse
            {
                Token = authenticate,
                Message = "Authenticated"
            };
                
        }

        [HttpPost("SignUp")]
        public SignupResponseDTO SignUp([FromBody] SignupRequestDTO request)
            {
            if(request!= null)
            {
                var user = new IdentityUser
                {
                    UserName = request.Name,
                    Email = request.Email,
                    NormalizedUserName = request.Name.ToUpper(),
                    NormalizedEmail = request.Email.ToUpper(),

                };
                
                var result = _userManager.CreateAsync(user, request.password).Result;
                if (result.Succeeded)
                {
                    return new SignupResponseDTO
                    {
                        Token = _tokenService.GenerateToken(user.Id, user.Email).Result,
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
                        Status = StatusCode(400).ToString()
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
    }
}

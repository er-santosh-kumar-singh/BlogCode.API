using BlogCode.API.Common;
using BlogCode.API.Models.DTO;
using BlogCode.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogCode.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        //POST {apibaseurl}/api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Check email
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser is not null)
            {
                // check password
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, request.Password);
                if (checkPasswordResult)
                {
                    //Create Token and Response
                    var roleFromDb = await _userManager.GetRolesAsync(identityUser);
                    // Call token respository                  
                   var jwtToken = _tokenRepository.CreateToken(identityUser, roleFromDb.ToList());

                    var response = new LoginResponseDto()
                    {
                        Email = identityUser.Email,
                        Roles = roleFromDb.ToList(),
                        Token = jwtToken
                    };
                    return Ok(response);
                }

            }
            ModelState.AddModelError("", "User id or password incorrect");
            return ValidationProblem(ModelState);
        }

        //POST {apibaseurl}/api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Create Identity User Object
            var user = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim(),
            };

            // Create the new user

            var identityResult = await _userManager.CreateAsync(user, request.Passowrd);
            if (identityResult.Succeeded)
            {
                // Assign the role
                identityResult = await _userManager.AddToRoleAsync(user, UserRoles.Reader);
                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }
    }
}

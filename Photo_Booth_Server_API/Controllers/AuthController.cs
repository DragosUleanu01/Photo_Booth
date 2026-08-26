using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Photo_Booth_Server_API.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Identity.Client;
using Photo_Booth_Server_API.DTO;

namespace Photo_Booth_Server_API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        
        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser != null)
            {
                return BadRequest("Email already registered");
            }

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(

                user,
                request.Password

                );

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User Registered successfully");
        }


    }
}

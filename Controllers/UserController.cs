using ASPNet_Authentication.Models;
using ASPNet_Authentication.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ASPNet_Authentication.Controllers
{
    [Route("api")]
    [ApiController]

    public class UserController : Controller
    {
        
        private readonly UserService service;
        private readonly IConfiguration _configuration;

        public UserController(UserService _service , IConfiguration configuration)
        {
            service = _service;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public ActionResult<User> signUp(User user)
        {
            var token = CreateToken(user);
            try
            {
                service.CreateUser(user);
                return StatusCode(200, new
                {
                    status = true,
                    token = token,
                    ExpiresIn = 3600
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }


        }

        [HttpGet , Authorize]
        public ActionResult<string> GetMe()
        {
            
            return Ok("asdasdasd");

            //var userName = User?.Identity?.Name;
            //var userName2 = User.FindFirstValue(ClaimTypes.Name);
            //var role = User.FindFirstValue(ClaimTypes.Role);
            //return Ok(new { userName, userName2, role });
        }

        [HttpPost("signin")]
        public ActionResult<User> login(User user)
        {
            try
            {
                var userData = service.GetUser(user.Email, user.Password);
                var token = CreateToken(user);
                if (userData != null)
                {   
                    
                    return StatusCode(200, new
                    {
                        status = true,
                        token = token,
                        ExpiresIn = 3600
                    });
                }
                else
                {
                    return StatusCode(401, new
                    {
                        status = false,
                        message = "Invalid Credentials"
                    });

                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtConfig:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
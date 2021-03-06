using ASPNet_Authentication.Models;
using ASPNet_Authentication.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Text;
namespace ASPNet_Authentication.Controllers
{
    [Route("api")]
    [ApiController]

    public class UserController : Controller
    {

        private readonly UserService service;
        private readonly IConfiguration _configuration;


        public UserController(UserService _service, IConfiguration configuration)
        {
            service = _service;
            _configuration = configuration;
        }

        [HttpPost("signup")]
        public ActionResult<User> signUp(User user)
        {
            // string value = Environment.GetEnvironmentVariable("TEST");
            var token = createToken(user);
            var code = codeGenerator();
            try
            {
                service.CreateUser(user);
                // sendEmail(code , user.Email);
                service.codeAdd(code, user.Email);
                return StatusCode(200, new
                {
                    status = true,
                    token = token,
                    ExpiresIn = 3600
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }


        }

        [HttpPost("verify_email"), Authorize]
        public ActionResult<Codes> verifyEmail(Codes code)
        {
            var me = Request.Headers["Authorization"].ToString();

            try
            {
                var codeData = service.GetCode(code.Code, code.Email);
                if (codeData == null)
                {
                    return StatusCode(401, new
                    {
                        status = false,
                        message = "Invalid code"
                    });
                }
                else
                {
                    return StatusCode(200, new
                    {
                        status = true,
                        message = "Email verified"
                    });
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("signin")]
        public ActionResult<User> login(User user)
        {
            // sendEmail();
            var code = codeGenerator();
            try
            {
                var userData = service.GetUser(user.Email, user.Password);
                var token = createToken(user);
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("forgot_password")]
        public ActionResult<User> forgotPassword(User user)
        {
            try
            {
                var responseDB = service.findUser(user.Email);

                if (responseDB)
                {
                    var code = codeGenerator();
                    var token = createToken(user);

                    try
                    {
                        service.forgotPassword(code, token, user.Email);
                        // sendEmail(code, user.Email);
                        return StatusCode(200, new
                        {
                            status = true,
                            message = "Reset code has been send to your email",
                            token = token
                        });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            status = false,
                            message = ex.Message
                        });
                    }

                }
                else
                {
                    return StatusCode(401, new
                    {
                        status = false,
                        message = "Email not found"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }


        }

        [HttpPost("verify_code"), Authorize]
        public ActionResult<ForgptPass> verifyCode(Codes code)
        {
            var head = Request.Headers["Authorization"].ToString();

            try
            {
                var res = service.verifyResetCode(head.Remove(0, 7), code.Code);

                if (res != null)
                {

                    return StatusCode(200, new
                    {
                        status = true,
                        message = "Code verified"
                    });
                }
                else
                {
                    return StatusCode(401, new
                    {
                        status = false,
                        message = "Invalid code"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = ex.Message
                });
            }

        }

        [HttpPost("new_password"), Authorize]

        public ActionResult<NewPassword> newPassword(NewPassword pass)
        {
            var head = Request.Headers["Authorization"].ToString();

            if (pass.Password == pass.Confirm)
            {
                try
                {
                    service.resetPassword(head.Remove(0, 7), pass.Password);

                    return StatusCode(200, new
                    {
                        status = true,
                        message = "Password changed"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = false,
                        message = ex.Message
                    });
                }
            }
            else
            {
                return StatusCode(400, new
                {
                    status = false,
                    message = "Password not match"
                });
            }


        }

        private string createToken(User user)
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

        private void sendEmail(string code, string email)
        {
            string to = email; //To address    
            string from = "from"; //From address    
            MailMessage message = new MailMessage(from, to);

            string mailbody = "Your verification code is " + code;
            message.Subject = "ASP.NET Authentication";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("from", "password");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string codeGenerator()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }
    }
}
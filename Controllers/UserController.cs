using ASPNet_Authentication.Models;
using ASPNet_Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASPNet_Authentication.Controllers
{
    [Route("api")]
    [ApiController]

    public class UserController : Controller
    {
        private readonly UserService service;

        public UserController(UserService _service)
        {
            service = _service;
        }

        [HttpGet("users")]
        public ActionResult<List<User>> GetUsers()
        {
            return service.GetUsers();
        }

        [HttpGet("users/{id}")]
        public ActionResult<User> GetUser(string id)
        {
            var user = service.GetUser(id);
            return Json(user);
        }

        [HttpPost("users")]
        public ActionResult<User> Create(User user)
        {
            try
            {
                service.CreateUser(user);
                return Json(new
                {
                    status = true,
                    message = "User created sucessfully"
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = "An Internal Error Occured"
                });
            }


        }
    }
}
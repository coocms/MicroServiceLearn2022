using Microserivce.Interface;
using Microservice.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.UserServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        
        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserController(IUserService userService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService; 
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize]//需要授权
        [HttpGet]
        public IActionResult GetUserAll()
        {
            Console.WriteLine($"This is UsersController {_configuration["port"] ?? _configuration["port"]} Invoke");
            var host = _httpContextAccessor.HttpContext!.Request.Host;

            return Ok(_userService.UserAll().Select(u => new User()
            {
                Id = u.Id,
                Account = u.Account + "MA",
                Name = u.Name,
                Role = $"{_configuration["ip"] ?? host.Host}" +
                $"{_configuration["port"] ?? (host.Port is null ? "NonePort" : host.Port!.Value.ToString())}",
                Email = u.Email,
                LoginTime = u.LoginTime,
                Password = u.Password + "K8S"
            }));
        }



    }
}

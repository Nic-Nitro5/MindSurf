using MindSurf.Data;
using MindSurf.Dtos;
using MindSurf.Models;
using Microsoft.AspNetCore.Mvc;
using MindSurf.Helpers;

namespace MindSurf.Controllers
{
    [Route(template:"api")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository repository, JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        [HttpPost(template:"register")]
        public IActionResult Register(RegisterDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            return Created("Success", _repository.Create(user));
        }

        [HttpPost(template:"login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _repository.GetByEmail(dto.Email);

            //Update user if remember me
            user.RememberMe = dto.RememberMe;
            _repository.UpdateUser(user);

            if(user == null)
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            if(!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            var jwt = _jwtService.Generate(user.Id, dto.RememberMe);

            Response.Cookies.Append("jwt", jwt, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true
            });

            return Ok(new
            {
                message = "Success"
            });
        }


        [HttpGet("user")]
        public IActionResult User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = _repository.GetById(userId);

                return Ok(user);
            }
            catch
            {
                return BadRequest(new { message = "unauthorized" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new
            {
                message = "Success"
            });
        }
    }
}

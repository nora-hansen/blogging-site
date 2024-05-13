using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogAPI.Models;


namespace BlogAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly BlogContext _context;

        public LoginController(IConfiguration config, BlogContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous] // Alows unauthenticated users to use this endpoint
        [HttpPost]
        public IActionResult Login([FromBody]UserLoginDTO login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            var currentUser = HttpContext.User;
            foreach (Claim claim in currentUser.Claims)
            {
                Console.WriteLine("CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + "</br>");
            }
            
            return new string[] { "value1", "value2", "value3", "value4", "value5" };

        }

        private string GenerateJSONWebToken(UserLoginDTO userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserLoginDTO AuthenticateUser(UserLoginDTO login)
        {
            UserLoginDTO user = null;
            var userSigningIn = _context.Users.Select(u =>
                new User()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password
                }).SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (userSigningIn != null)
            {
                user = new UserLoginDTO { Email = userSigningIn.Email };
            }

            return user;
        }
    }
}
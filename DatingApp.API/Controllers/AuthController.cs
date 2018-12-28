using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IConfiguration config, IMapper mapper)
        {
            _authRepository = authRepository;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)  // ApiController annotation automatically tells this method to infer that this param is in the body of the request
        {
            // ApiController also adds for this functionality for us
            // if (!ModelState.IsValid) return BadRequest(ModelState)
            // Which runs the model validation and returns the error messages

            // Validate request
            var username = userForRegisterDto.Username.ToLower();
            if (await _authRepository.UserExists(username))
                return BadRequest("User already exists");
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _authRepository.Register(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);
            return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) 
        {
            // throw new Exception("Humans Are Not Allowed In The Dog Park");
            // Validate request
            var username = userForLoginDto.Username.ToLower();
            var loggedInUser = await _authRepository.Login(username, userForLoginDto.Password);

            if (loggedInUser == null)
                return Unauthorized();

            // Build JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
                new Claim(ClaimTypes.Name, loggedInUser.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));// Sign the token with our server secret token

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // Create signing credentials with the security key

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var authenticatedUser = _mapper.Map<UserForListDto>(loggedInUser);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user = authenticatedUser
            });
        }
    }
}
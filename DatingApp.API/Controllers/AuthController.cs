using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            // IAuthRepository authRepository,
            IConfiguration config,
            IMapper mapper,
            UserManager<User> userManager, SignInManager<User> signInManager)
        {
            // _authRepository = authRepository;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)  // ApiController annotation automatically tells this method to infer that this param is in the body of the request
        {
            // ApiController also adds for this functionality for us
            // if (!ModelState.IsValid) return BadRequest(ModelState)
            // Which runs the model validation and returns the error messages

            // Validate request
            // var username = userForRegisterDto.Username.ToLower();
            // if (await _authRepository.UserExists(username))
            //     return BadRequest("User already exists");
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            // var createdUser = await _authRepository.Register(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailDto>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) 
        {
            // throw new Exception("Humans Are Not Allowed In The Dog Park");
            // Validate request
            // var username = userForLoginDto.Username.ToLower();
            // var loggedInUser = await _authRepository.Login(username, userForLoginDto.Password);

            var loggedInUser = await _userManager.FindByNameAsync(userForLoginDto.Username);

            if (loggedInUser == null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(loggedInUser, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
                var authenticatedUser = _mapper.Map<UserForListDto>(appUser);

                return Ok(new {
                    token = GenerateToken(appUser).Result,
                    user = authenticatedUser
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateToken(User loggedInUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
                new Claim(ClaimTypes.Name, loggedInUser.UserName)
            };

            var roles = await _userManager.GetRolesAsync(loggedInUser);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }
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
            return tokenHandler.WriteToken(token);
        }
    }
}
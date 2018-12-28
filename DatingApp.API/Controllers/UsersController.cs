using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repository.GetUsers();
            var mappedUsers = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(mappedUsers);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var mappedUser = _mapper.Map<UserForDetailDto>(user);
            return Ok(mappedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserFormDto userFormDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Makes sure the user that is being edited is the person who is logged in
                return Unauthorized();
            var currentUser = await _repository.GetUser(id);
            _mapper.Map(userFormDto, currentUser); // Update just the matching values between these objects
            return await _repository.SaveAll() ? NoContent() : throw new Exception($"Updating user {id} failed on save.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // This action filter will run on all endpoints here
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
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var currentUser = await _repository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "female" ? "male" : "female";
            }
            PagedList<User> users = await _repository.GetUsers(userParams);
            var mappedUsers = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.CurrentPage);
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

        [HttpPost("{likerId}/like/{likeeId}")]
        public async Task<IActionResult> LikeUser(int likerId, int likeeId)
        {
            if (likerId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // Makes sure the user that is being edited is the person who is logged in
                return Unauthorized();
            
            var like = await _repository.GetLike(likerId, likeeId);

            if (like != null) // Or could we just delete it?
                return BadRequest("You already like this user");

            if (await _repository.GetUser(likeeId) == null)
                return NotFound();
            
            like = new Like
            {
                LikerId = likerId,
                LikeeId = likeeId,
            };

            _repository.Add<Like>(like);
            if (await _repository.SaveAll())
                return Ok();
            return BadRequest("Failed to like User");
        }
    }
}
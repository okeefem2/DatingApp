using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Identity;
using DatingApp.API.Models;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using DatingApp.API.helpers;
using CloudinaryDotNet.Actions;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;
            _userManager = userManager;
            _cloudinaryConfig = cloudinaryConfig;
            Account cloudinaryAccount = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(cloudinaryAccount);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await (
                from user in _context.Users orderby user.UserName
                select new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Roles =
                    (
                        from userRole in user.UserRoles
                        join role in _context.Roles 
                        on userRole.RoleId equals role.Id
                        select role.Name
                    ).ToList()
                }
            ).ToListAsync();
            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleFormDto roleFormDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleFormDto.RoleNames ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); // Add only new roles

            if (!result.Succeeded)
                return BadRequest("Failed to add roles for user");
            
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove roles for user");
            
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos
                .Include(p => p.User)
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(p => new
                    {
                        Id = p.Id,
                        UserName = p.User.UserName,
                        Url = p.Url,
                        IsApproved = p.IsApproved
                    }).ToListAsync();
            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);
            photo.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("denyPhoto/{photoId}")]
        public async Task<IActionResult> DenyPhoto(int photoId)
        {
            var photo = await _context.Photos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo.IsMain)
                return BadRequest("Cannot reject main photo");
            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result != "ok")
                {
                    return BadRequest("Could not delete from Cloundinary");
                }
            }
            _context.Remove(photo);

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
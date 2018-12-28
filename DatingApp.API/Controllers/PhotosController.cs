using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        public PhotosController(IDatingRepository repository,
                                IMapper mapper,
                                IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repository = repository;
            Account cloudinaryAccount = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(cloudinaryAccount);
        }

        [HttpGet("{photoId}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int photoId)
        {
            Photo photo = await _repository.GetPhoto(photoId);

            PhotoForDetailDto photoDto = _mapper.Map<PhotoForDetailDto>(photo);
            return Ok(photoDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoFormDto photoDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User currentUser = await _repository.GetUser(userId);

            IFormFile file = photoDto.File;

            ImageUploadResult uploadResult = new ImageUploadResult();

            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream()) // using allows us to dispose of the file in memory when done
                {
                    ImageUploadParams uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
                photoDto.Url = uploadResult.Uri.ToString();
                photoDto.PublicId = uploadResult.PublicId;
                Photo photoToSave = _mapper.Map<Photo>(photoDto);

                if (currentUser.Photos.Any(photo => photo.IsMain))
                {
                    photoToSave.IsMain = true;
                }
                currentUser.Photos.Add(photoToSave);

                if (await _repository.SaveAll())
                {
                    PhotoForDetailDto photoForDetail = _mapper.Map<PhotoForDetailDto>(photoToSave);
                    return CreatedAtRoute("GetPhoto", new { photoId = photoForDetail.Id }, photoForDetail);
                }
            }
            return BadRequest("Photo could not be saved");
        }

        [HttpPost("{photoId}/setMain")]
        public async Task<IActionResult> SetMain(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User currentUser = await _repository.GetUser(userId);
            Photo newMainPhoto = currentUser.Photos.FirstOrDefault(p => p.Id == photoId);
            if (newMainPhoto == null)
                return Unauthorized();
            
            if (newMainPhoto.IsMain)
                return BadRequest("Photo is already main");

            Photo oldMainPhoto = currentUser.Photos.FirstOrDefault(p => p.IsMain);

            oldMainPhoto.IsMain = false;
            newMainPhoto.IsMain = true;

            if (await _repository.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Could not set the new main");
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User currentUser = await _repository.GetUser(userId);
            Photo photoToDelete = currentUser.Photos.FirstOrDefault(p => p.Id == photoId);
            
            if (photoToDelete.IsMain)
                return BadRequest("Cannot delete main photo! Please add a replacement and set that to main first");
            var deletetionParams = new DeletionParams(photoToDelete.PublicId);
            var result = _cloudinary.Destroy(deletetionParams);

            if (result.Result == "ok")
            {
                _repository.Delete(photoToDelete);
                if (await _repository.SaveAll())
                {
                    return Ok();
                }
            }
            
            return BadRequest("Could not delete photo");
        }
    }
}
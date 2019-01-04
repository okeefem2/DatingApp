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
    // [Authorize] Can remove now that identity auths globally
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messages = await _repository.GetMessageThread(userId, recipientId);

            if (messages == null)
                return NotFound();
            var messageThread = _mapper.Map<IEnumerable<MessageForDetailDto>>(messages);

            return Ok(messageThread);
        }

        [HttpGet("{messageId}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int messageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            Message message = await _repository.GetMessage(messageId);

            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            
            var messages = await _repository.GetMessagesForUser(messageParams);

            var messagesToReturn = _mapper.Map<IEnumerable<MessageForDetailDto>>(messages);

            Response.AddPagination(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messagesToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessageForUser(int userId, MessageFormDto messageDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            User currentUser = await _repository.GetUser(userId);

            messageDto.SenderId = userId;

            User recipient = await _repository.GetUser(messageDto.RecipientId);

            if (recipient == null)
                return BadRequest("Recipient does not exist");

            Message message = _mapper.Map<Message>(messageDto);
            _repository.Add(message);

            if (await _repository.SaveAll())
            {
                MessageForDetailDto messageToReturnDto = _mapper.Map<MessageForDetailDto>(message);

                return CreatedAtRoute("GetMessage", new { messageId = message.Id }, messageToReturnDto);
            }
       
            return BadRequest("Message could not be saved");
        }

        [HttpPost("{messageId}")]
        public async Task<IActionResult> DeleteForUser(int userId, int messageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            Message messageToDelete = await _repository.GetMessage(messageId);
            if (messageToDelete != null)
            {
                messageToDelete.SenderDeleted = messageToDelete.SenderId == userId;
                messageToDelete.RecipientDeleted = messageToDelete.RecipientId == userId;

                if (messageToDelete.SenderDeleted && messageToDelete.RecipientDeleted)
                    _repository.Delete(messageToDelete);
                
                if (await _repository.SaveAll())
                {
                    return NoContent();
                }
            }
            return BadRequest("Message could not be deleted");
        }

        [HttpPost("read/{messageId}")]
        public async Task<IActionResult> MessageRead(int userId, int messageId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            Message messageRead = await _repository.GetMessage(messageId);
            if (messageRead != null)
            {
                messageRead.IsRead = true;
                messageRead.DateRead = DateTime.Now;

                if (await _repository.SaveAll())
                {
                    return NoContent();
                }
            }
            return BadRequest("Message could not be read");
        }
    }

    
}
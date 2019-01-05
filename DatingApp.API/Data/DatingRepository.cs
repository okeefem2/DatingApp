using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity); // Saved in memory not in db yet
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            // Photos are a navigation property, so use Include to include them
            var query = _context.Users.Include(u => u.Photos).AsQueryable();

            if(isCurrentUser)
                query = query.IgnoreQueryFilters();
            var user = await query.FirstOrDefaultAsync(u => u.Id == id);
            
            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            // Photos are a navigation property, so use Include to include them
            var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var usersQuery = _context.Users.Include(u => u.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            usersQuery = usersQuery.Where(u => u.Id != userParams.UserId && u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                usersQuery = usersQuery.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                usersQuery = usersQuery.Where(u => userLikees.Contains(u.Id));            
            }

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            usersQuery = usersQuery.Where(u => u.birthDate >= minDob && u.birthDate <= maxDob);

            if (!string.IsNullOrEmpty((userParams.OrderBy)))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        usersQuery = usersQuery.OrderByDescending(u => u.Created);
                        break;
                    default:
                        usersQuery = usersQuery.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(usersQuery, userParams);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Like> GetLike(int likerId, int likeeId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == likerId && u.LikeeId == likeeId);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int userId, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likers).Include(x => x.Likees).FirstOrDefaultAsync(u => u.Id == userId);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == userId).Select(l => l.LikerId); // Return list of liker Ids
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == userId).Select(l => l.LikeeId); // Return list of liker Ids
            }

        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos).AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted);
                    break;
                case "outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId  && !m.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted && !m.IsRead);
                    break;
            }
            
            messages = messages.OrderByDescending(m => m.DateSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int senderId, int recipientId)
        {
            // For performance it would be better to just grab the messages, then query once for the sender and recip and attach the info needed?
            return await _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .Where(m => (m.RecipientId == recipientId && m.SenderId == senderId && !m.RecipientDeleted) 
                    || (m.RecipientId == senderId && m.SenderId == recipientId && !m.SenderDeleted)
                )
                .OrderByDescending(m => m.DateSent).ToListAsync();
        }
    }
}
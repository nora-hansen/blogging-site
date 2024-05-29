using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFriendController : ControllerBase
    {
        private readonly BlogContext _context;

        public UserFriendController(BlogContext context)
        {
            _context = context;
        }

        /**
         * Get one user's friends. Currently two entries per friendship... Other way?
         */
        [HttpGet("{id}")]
        public IQueryable<UserFriend> GetFriendsOfUser(int id)
        {
            var friends = from f in _context.UserFriend
                          where f.UserId == id
                          select new UserFriend()
                          {
                              UserId = f.UserId,
                              FriendId = f.FriendId,
                          };
            return friends;
        }

        /**
         * Create a new friendship. A bit messy
         * TODO: Make this better
         *  Check if the user accepting is the one logged in
         */
        [HttpPost]
        public async Task<ActionResult<UserFriend>> PostFriendship(UserFriend userFriend)
        {
            // What does this actually do?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!UserExists(userFriend.UserId) || !UserExists(userFriend.FriendId))
            {
                return BadRequest("Invalid user!");
            }

            var existingFriendship = from fr in _context.UserFriend
                                     where fr.UserId == userFriend.UserId && fr.FriendId == userFriend.FriendId
                                     select new UserFriend();

            if (FriendshipExists(userFriend.UserId, userFriend.FriendId) || FriendshipExists(userFriend.FriendId, userFriend.UserId))
            {
                return BadRequest("Already friends!");
            }

            _context.UserFriend.Add(userFriend);

            _context.UserFriend.Add(new UserFriend()
            {
                UserId = userFriend.FriendId,
                FriendId = userFriend.UserId,
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostFriendship), new { userFriend });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendship(int userId, int friendId)
        {
            var friendship = await _context.UserFriend.FindAsync(userId, friendId);
            if (friendship == null)
            {
                return NotFound();
            }

            _context.UserFriend.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool FriendshipExists(int userId, int friendId)
        {
            var existingFriendship = from fr in _context.UserFriend
                                 where fr.UserId == userId && fr.FriendId == friendId
                                 select new UserFriend();

            return existingFriendship.Any();
        }
    }
}
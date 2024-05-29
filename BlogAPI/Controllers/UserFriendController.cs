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
         */
        [HttpPost]
        public async Task<ActionResult<UserFriend>> PostFriendship(UserFriend userFriend)
        {
            // What does this actually do?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user1 = from u in _context.Users
                        where u.Id == userFriend.UserId
                        select new User();

            var user2 = from u in _context.Users
                        where u.Id == userFriend.FriendId
                        select new User();

            if (!user1.Any() || !user2.Any())
            {
                return BadRequest("Invalid user!");
            }

            var existingFriendship = from fr in _context.UserFriend
                                     where fr.UserId == userFriend.UserId && fr.FriendId == userFriend.FriendId
                                     select new UserFriend();

            if (existingFriendship.Any())
            {
                return BadRequest("Already friends!");
            }

            existingFriendship = from fr in _context.UserFriend
                                     where fr.FriendId == userFriend.UserId && fr.UserId == userFriend.FriendId
                                     select new UserFriend();

            if (existingFriendship.Any())
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

            return CreatedAtAction(nameof(PostFriendship), new { user1 });
        }
    }
}
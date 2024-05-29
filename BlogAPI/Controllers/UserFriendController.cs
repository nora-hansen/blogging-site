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
         * Get one user's friends
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
    }
}
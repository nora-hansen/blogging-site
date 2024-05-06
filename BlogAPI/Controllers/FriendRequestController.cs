using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;


namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestsController : ControllerBase
    {
        private readonly BlogContext _context;

        public FriendRequestsController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FriendRequest>> GetTodoItem(int id)
        {
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.RecipientId == id)
                .ToListAsync();

            return Ok(friendRequests);
        }

        [HttpPost]
        public async Task<ActionResult<FriendRequest>> SendFriendRequest(FriendRequest friendRequest) 
        {
            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(SendFriendRequest), new { SenderId = friendRequest.SenderId, RecipientId = friendRequest.RecipientId }, friendRequest);
        }
    }
}
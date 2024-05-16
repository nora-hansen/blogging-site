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
        public async Task<ActionResult<FriendRequest>> GetFriendRequests(int id)
        {
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.RecipientId == id)
                .ToListAsync();

            return Ok(friendRequests);
        }

        /**
         * Get request from specific sender to specific recipient
         */
        [HttpGet]
        public async Task<ActionResult<FriendRequest>> GetSpecificRequestIfExists(int senderId, int recipientId)
        {
            var friendRequest = await _context.FriendRequests.Select(fr =>
                new FriendRequest()
                {
                    SenderId = fr.SenderId,
                    RecipientId = fr.RecipientId
                }).SingleOrDefaultAsync(fr => fr.RecipientId == recipientId && fr.SenderId == senderId);
            if (friendRequest == null)
            {
                return NotFound();
            }

            return Ok(friendRequest);
        
        }

        [HttpPost]
        public async Task<ActionResult<FriendRequest>> SendFriendRequest(FriendRequest friendRequest) 
        {
            var existingFriend = await _context.FriendRequests.Select(fr =>
                            new FriendRequest()
                            {
                                SenderId = fr.SenderId,
                                RecipientId = fr.RecipientId
                            }).SingleOrDefaultAsync(fr => fr.RecipientId == friendRequest.RecipientId && fr.SenderId == friendRequest.SenderId);
            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            if (existingFriend != null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(SendFriendRequest), new { SenderId = friendRequest.SenderId, RecipientId = friendRequest.RecipientId }, friendRequest);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFriendRequest(int senderId, int recipientId)
        {
            var friendRequest = await _context.FriendRequests.Select(fr =>
                            new FriendRequest()
                            {
                                SenderId = fr.SenderId,
                                RecipientId = fr.RecipientId
                            }).SingleOrDefaultAsync(fr => fr.RecipientId == recipientId && fr.SenderId == senderId);
            if (friendRequest == null)
            {
                return NotFound();
            }

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
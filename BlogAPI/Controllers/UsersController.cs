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
    public class UsersController : ControllerBase
    {
        private readonly BlogContext _context;

        public UsersController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IQueryable<UserDTO>>> GetUsers()
        {
            var users = from u in _context.Users
                        select new UserDTO()
                        {
                            Id = u.Id,
                            Email = u.Email,
                            DisplayName = u.DisplayName,
                            IconUrl = u.IconUrl
                        };
            return Ok(users);
        }

        // GET: api/users/1
        /*
         * 
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id, bool includePosts)
        {
            User user = null;
            if (includePosts == true)   // Include the users posts in the response body?
            {
                user = await _context.Users.Select(u =>
                new User()
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    IconUrl = u.IconUrl,
                    Posts = u.Posts,
                }).SingleOrDefaultAsync(u => u.Id == id);
            }
            else
            {
                user = await _context.Users.Select(u =>
                    new User()
                    {
                        Id = u.Id,
                        Email = u.Email,
                        DisplayName = u.DisplayName,
                        IconUrl = u.IconUrl,
                    }
                ).SingleOrDefaultAsync(u => u.Id == id);
            }   

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // What does this actually do?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(User), new { id = user.Id }, user);
        }

        // PUT: api/users/1
        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(User user, int id)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/users/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
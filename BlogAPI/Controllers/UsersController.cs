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
    public class UsersController : ControllerBase
    {
        private readonly BlogContext _context;

        public UsersController(BlogContext context)
        {
            _context = context;
        }

        /**
         * Gets all the users in the database
         * TODO: This was previously async. Should there be an await here? 
         *  There is no await in the MS learn page
         */
        [HttpGet]
        public IQueryable<UserDTO> GetUsers(string? email)
        {
            if(email is null)
            {   var users = from u in _context.Users
                select new UserDTO()
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    IconUrl = u.IconUrl,
                    ProfileId = u.ProfileId
                };
                return users;
            }
            else
            {
                var users = from u in _context.Users
                            where u.Email == email
                            select new UserDTO()
                            {
                                Id = u.Id,
                                Email = u.Email,
                                DisplayName = u.DisplayName,
                                IconUrl = u.IconUrl,
                                ProfileId = u.ProfileId
                            };
                return users;
            }
        }

        /*
         * Get a single user in the database
         * id - ID of the user - Required
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.Select(u =>
                new UserDTO()
                {
                    Id = u.Id,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    IconUrl = u.IconUrl,
                    Posts = u.Posts,
                    ProfileId = u.ProfileId

                }).SingleOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /**
         * Create a new user
         * Requestbody:
         * {
         *  email: string // Required
         *  password: string // Required
         *  displayName: string // Required
         * }
         */
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // What does this actually do?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEmail = from u in _context.Users
                                where u.Email == user.Email
                                select new User();

            Console.WriteLine(existingEmail);

            if (existingEmail.Any()) 
            {
                return BadRequest("A user with this email already exists!");
            }

            var profileSettings = new Profile();
            user.Profile = profileSettings;
            user.ProfileId = profileSettings.Id;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IconUrl = user.IconUrl,
            };

            return CreatedAtAction(nameof(PostUser), new { id = user.Id }, dto);
        }

        /**
         * Edit a user. TODO: Not tested
         */
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(User user, int id)
        {
            //if (id != user.Id)
            //{
            //    return BadRequest();
            //}

            User? originalUser = _context.Users.FirstOrDefault(u => u.Id == id);
            if (originalUser == null)
                return NotFound();

            if (user.DisplayName != originalUser.DisplayName) originalUser.DisplayName = user.DisplayName;
            if (user.Email != originalUser.Email) originalUser.Email = user.Email;
            if (user.IconUrl != originalUser.IconUrl) originalUser.IconUrl = user.IconUrl;

            _context.Entry(originalUser).State = EntityState.Modified;

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

        /**
         * TODO: Not tested
         */
        [Authorize]
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
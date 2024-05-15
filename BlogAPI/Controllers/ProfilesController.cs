using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly BlogContext _context;

        public ProfilesController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileDTO>> GetProfile(int id)
        {
            var profile = await _context.Profiles.Select(p =>
            new ProfileDTO()
            {
                Id = p.Id,
                bgColor = p.bgColor,
                fontColor = p.fontColor,
                postColor = p.postColor,
            }).SingleOrDefaultAsync(p  => p.Id == id);

            if(profile == null)
            {
                return NotFound();
            }
            return profile;
        }

        /*
         * Update a profile
         */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, Profile profile)
        {
            var currentUser = HttpContext.User;

            var profileOwner = _context.Users.SingleOrDefault(u => u.Email == currentUser.FindFirstValue(ClaimTypes.Email));
            if(profileOwner == null)
            {
                return Unauthorized();
            }

            var profileToUpdate = await _context.Profiles.
                Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (profileToUpdate == null)
            {
                return NotFound("No userprofile found :(");
            }

            if (profileToUpdate.bgColor != profile.bgColor && profile.bgColor!= null && profile.bgColor != "") profileToUpdate.bgColor = profile.bgColor;
            if (profileToUpdate.fontColor != profile.fontColor && profile.fontColor != null && profile.fontColor != "") profileToUpdate.fontColor = profile.fontColor;  
            if (profileToUpdate.postColor != profile.postColor && profile.postColor != null && profile.postColor != "") profileToUpdate.postColor = profile.postColor;

            _context.Entry(profileToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }
        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
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

        // TODO: Post
    }
}
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


/*
 *  TODO: Test all endpoints
 *  LEFT: Put
 */
namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BlogContext _context;

        public PostsController(BlogContext context)
        {
            _context = context;
        }

        /**
         * Get all the posts in the database
         */
        [HttpGet]
        public IQueryable<PostDTO> GetPosts(int? userID, bool? isDraft)
        {
            if (isDraft != null && userID != null)
            {
                var posts = from p in _context.Posts
                            where p.UserID == userID && p.IsDraft == true
                            select new PostDTO()
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Content = p.Content,
                                PostDate = p.PostDate,
                                UserID = p.UserID,
                                IsDraft = p.IsDraft,
                                Comments = p.Comments
                            };
                return posts;
            }
            else if (userID != null) 
            {
                var posts = from p in _context.Posts
                            where p.UserID == userID
                            select new PostDTO()
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Content = p.Content,
                                PostDate = p.PostDate,
                                UserID = p.UserID,
                                IsDraft = p.IsDraft,
                                Comments = p.Comments
                            };
                return posts;
            }
            else 
            {
                var posts = from p in _context.Posts
                            where p.IsDraft == false
                            select new PostDTO()
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Content = p.Content,
                                PostDate = p.PostDate,
                                UserID = p.UserID,
                                IsDraft = p.IsDraft,
                                Comments = p.Comments
                            };
                return posts;
            }
        }

        /**
         * Get single post from the database
         * id - The post ID, int. Required
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id, bool includeComments)
        {
            var post = await _context.Posts.Select(p =>
                new PostDTO()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    PostDate = p.PostDate,
                    UserID = p.UserID,
                    Visibility = p.Visibility,
                    IsDraft = p.IsDraft
                }).SingleOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        /**
         * Create a new post. 
         * Requestbody:
         * {
         *  userID: int // Required
         *  title: string
         *  content: string
         * }
         */
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostDTO>> PostPost(Post post)
        {
            var currentUser = HttpContext.User;

            // TODO: What does this do?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postingUser = _context.Users.SingleOrDefault(u => u.Email == currentUser.FindFirstValue(ClaimTypes.Email));
            if (postingUser == null)
            {
                return Unauthorized();
            }

            post.User = postingUser;

            if(!post.IsDraft)
            {
                post.PostDate = DateTime.UtcNow;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var dto = new PostDTO()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                PostDate = post.PostDate,
                Visibility = post.Visibility,
                IsDraft = post.IsDraft,
            };

            /*
             * TODO: Fix this. The posts are in fact posted, but the response is an error
             */
            return CreatedAtAction(nameof(PostPost), new { id = post.Id }, dto);
        }

        // TODO: This endpoint is not tested
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            var currentUser = HttpContext.User;

            var postingUser = _context.Users.SingleOrDefault(u => u.Email == currentUser.FindFirstValue(ClaimTypes.Email));
            if (postingUser == null)
            {
                return Unauthorized();
            }

            var originalPost = await _context.Posts.
                Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (originalPost == null)
            {
                return NotFound("The post to be updated was not found :(");
            }

            if (post.Title != "" && post.Title != null) originalPost.Title = post.Title;
            if (post.Content != "" && post.Content != null) originalPost.Content = post.Content;
            originalPost.Visibility = post.Visibility;
            if (originalPost.IsDraft && !post.IsDraft) originalPost.PostDate = DateTime.UtcNow;
            if (originalPost.IsDraft && post.IsDraft != originalPost.IsDraft) originalPost.IsDraft = post.IsDraft;

            _context.Entry(originalPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var currentUser = HttpContext.User;

            var postingUser = _context.Users.SingleOrDefault(u => u.Email == currentUser.FindFirstValue(ClaimTypes.Email));
            if (postingUser == null)
            {
                return Unauthorized();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if(post.UserID != postingUser.Id)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;


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
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int userID)
        {
            var posts = from p in _context.Posts
                        select new PostDTO()
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Content = p.Content,
                            PostDate = p.PostDate,
                            UserID = p.UserID
                        };
            return Ok(posts);
        }

        /**
         * Get single post from the database
         * id - The post ID, int. Required
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.Include(p => p.Id).Select(p =>
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
        [HttpPost]
        public async Task<ActionResult<PostDTO>> PostPost(Post post)
        {
            // TODO: What does this do?
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postingUser = _context.Users.SingleOrDefault(u => u.Id == post.UserID);
            if (postingUser == null)
            {
                return NotFound("Invalid user!");
            }
            
            post.User = postingUser;
            Console.WriteLine("The post" + post);

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
            return CreatedAtAction(nameof(Post), new { id = post.Id }, dto);
        }

        // TODO: This endpoint is not tested
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly BlogContext _context;

        public CommentsController(BlogContext context)
        {
            _context = context;
        }

        /**
         * TODO: Not tested
         */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
            var comments = from c in _context.Comments
                           select new CommentDTO()
                           {
                               Id = c.Id,
                               Content = c.Content,
                               UserID = c.UserID,
                               PostID = c.PostID,
                           };
            return Ok(comments);

        }

        /**
         * TODO: Not tested
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.Select(c =>
            new CommentDTO()
            {
                Id = c.Id,
                Content = c.Content,
                UserID = c.UserID,
                PostID = c.PostID,
            }).SingleOrDefaultAsync(c => c.Id == id);

            if(comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        /**
         *  TODO: Post endpoint. Only userID and postID should be necessary, not the entire User object and post object
         */
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            var commentingUser = _context.Users.SingleOrDefault(u => u.Id == comment.UserID);
            if (commentingUser == null)
            {
                return NotFound("Invalid userID!");
            }
            var originalPost = _context.Posts.SingleOrDefault(p => p.Id == comment.PostID);
            if (originalPost == null)
            {
                return NotFound("Invalid postID!");
            }

            _context.Comments.Add(comment);
            comment.CommentingUser = commentingUser;
            comment.OriginalPost = originalPost;
            await _context.SaveChangesAsync();

            var dto = new CommentDTO()
            {
                Id = comment.Id,
                Content = comment.Content,
                CommentDate = comment.CommentDate,
                UserID = comment.UserID,
                PostID = comment.PostID
            };

            return CreatedAtAction(nameof(Comment), new { id = comment.Id }, dto);
        }

        /**
         * TODO: Not tested. Should only require userID and postID
         */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace BlogAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CommentDate { get; set; }
        public int UserID { get; set; }
        [Required]
        public User? CommentingUser { get; set; }
        public int PostID { get; set; }
        [Required]
        public Post? OriginalPost { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Comment()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
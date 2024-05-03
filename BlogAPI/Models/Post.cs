using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace BlogAPI.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public int UserID { get; set; }
        [Required]
        public User? User { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Post()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
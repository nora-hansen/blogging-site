using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public DateTime PostDate { get; set; }
        public int UserID { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Public;
        public bool IsDraft { get; set; } = true;
        public ICollection<Comment>? Comments { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PostDTO()
        {
            PostDate  = DateTime.Now;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
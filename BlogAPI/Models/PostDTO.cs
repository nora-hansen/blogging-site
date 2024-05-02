using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public int UserID { get; set; }

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
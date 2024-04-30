using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string? IconUrl { get; set; }
        public ICollection<User> Friends { get; set; }
        // //public ICollection<User> Blocks { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
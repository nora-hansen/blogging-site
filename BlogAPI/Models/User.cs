using Microsoft.Build.Framework.Profiler;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        public string? IconUrl { get; set; }
        public ICollection<UserFriend>? Friends { get; set; } = new List<UserFriend>();
        // Maybe in the future // public ICollection<User> Blocks { get; set; }
        
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        public int ProfileId { get; set; }
        public Profile? Profile { get; set; }

        public User()
        {

        }
    }
}
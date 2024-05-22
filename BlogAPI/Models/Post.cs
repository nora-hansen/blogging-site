using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;
using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public enum Visibility
    {
        Public,
        FriendsOnly,
        Private
    }
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PostDate { get; set; }
        public int UserID { get; set; }
        [Required]
        [JsonIgnore]
        public User? User { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Public;
        public bool IsDraft { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Post()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Post Copy()
        {
            Post copy = new Post();

            copy.Id = Id;
            copy.Title = Title;
            copy.Content = Content;
            copy.PostDate = PostDate;
            copy.User = User;
            copy.Comments = Comments;
            copy.Visibility = Visibility;
            copy.IsDraft    = IsDraft;
            copy.CreatedAt = CreatedAt;
            copy.UpdatedAt = UpdatedAt;

            return copy;
        }
    }
}
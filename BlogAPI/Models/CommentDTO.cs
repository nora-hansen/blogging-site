namespace BlogAPI.Models
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CommentDate { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
    }
}
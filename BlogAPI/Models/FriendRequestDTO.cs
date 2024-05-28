namespace BlogAPI.Models
{
    public class FriendRequestDTO
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderIconUrl { get; set; } = string.Empty;
    }
}
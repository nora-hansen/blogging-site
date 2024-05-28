namespace BlogAPI.Models
{
    public class FriendRequest
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public User Sender { get; set; }
    }
}
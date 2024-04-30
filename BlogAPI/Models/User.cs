namespace BlogAPI.Models
{
    public class User
    {
        private int Id { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private string displayName { get; set; }
        private string? iconUrl { get; set; }
        /*
        private List<User> friends { get; set; }
        private List<User> blocks { get; set; }
        private List<Post> posts { get; set; }
        private List<Comment> comments { get; set; }
        */
}
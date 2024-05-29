using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;

namespace BlogAPI.Models
{
	public class BlogContext : DbContext
	{
		private string _connectionString;

		public BlogContext(DbContextOptions<BlogContext> options) : base(options)
		{
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			_connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnectionString")!;
			this.Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(_connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Post>()
				.HasOne(e => e.User)
				.WithMany(e => e.Posts)
				.HasForeignKey(e => e.UserID);

			modelBuilder.Entity<Comment>()
				.HasOne(e => e.OriginalPost)
				.WithMany(e => e.Comments)
				.HasForeignKey(e => e.PostID);
		    
		    modelBuilder.Entity<Comment>()
		    	.HasOne(e => e.CommentingUser)
		    	.WithMany(e => e.Comments)
		    	.HasForeignKey(e => e.UserID);

            modelBuilder.Entity<User>()
                .HasOne(e => e.Profile);

            modelBuilder.Entity<UserFriend>(b =>
			{
				b.HasKey(uf => new { uf.UserId, uf.FriendId });
				b.HasOne(uf => uf.User).WithMany(uf => uf.Friends);
				b.HasOne(uf => uf.Friend).WithMany().OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<FriendRequest>()
				.HasKey(fr => new { fr.SenderId, fr.RecipientId });
        }

        public DbSet<User> Users { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<FriendRequest> FriendRequests { get; set; }
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<UserFriend> UserFriend { get; set; }
	}
}

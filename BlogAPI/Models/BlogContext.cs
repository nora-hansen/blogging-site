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
				.HasMany(e => e.Friends)
				.WithMany();
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
	}
}
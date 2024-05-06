using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace BlogAPI.Models
{
	public class Profile
	{
		public int Id { get; set; }
		public int userID { get; set; }
		public User Usr { get; set; }
		public string? bgColor { get; set; }
		public string? fontColor { get; set; }
		public string? postColor { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastUpdated { get; set; }

		public Profile()
		{
			CreatedAt = DateTime.UtcNow;
			LastUpdated = DateTime.UtcNow;
		}
	}
}
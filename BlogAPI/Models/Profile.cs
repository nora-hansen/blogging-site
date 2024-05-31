using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace BlogAPI.Models
{
	public class Profile
	{
		public int Id { get; set; }
		public string? bgColor { get; set; } = null;
		public string? fontColor { get; set; } = null;
		public string? postColor { get; set; } = null;
        public string? bio { get; set; } = null;
        public DateTime CreatedAt { get; set; }
		public DateTime LastUpdated { get; set; }

		public Profile()
		{
			CreatedAt = DateTime.UtcNow;
			LastUpdated = DateTime.UtcNow;
		}
	}
}
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace BlogAPI.Models
{
    public class ProfileDTO
    {
        public int Id { get; set; }
        public string? bgColor { get; set; } = null;
        public string? fontColor { get; set; } = null;
        public string? postColor { get; set; } = null;
    }
}
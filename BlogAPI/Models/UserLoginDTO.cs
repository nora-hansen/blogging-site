using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Models
{
    public class UserLoginDTO
    {
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace AuthMEANORM.Models.UsersModel
{
    public class Users
    {
        public ObjectId Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserPass { get; set; } = null!;

        public List<string>? UserRoles { get; set; } = new List<string> { "user" };

        public bool? IsActive { get; set; } = true;

        public string? Token { get; set; }
    }
}

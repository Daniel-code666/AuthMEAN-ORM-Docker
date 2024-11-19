using System.ComponentModel.DataAnnotations;

namespace AuthMEANORM.Models.UsersModel
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserPass { get; set; } = null!;
    }
}

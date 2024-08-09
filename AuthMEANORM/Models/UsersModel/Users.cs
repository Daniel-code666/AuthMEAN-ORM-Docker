using MongoDB.Bson;

namespace AuthMEANORM.Models.UsersModel
{
    public class Users
    {
        public ObjectId Id { get; set; }

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string UserPass { get; set; } = null!;

        public string UserRole { get; set; } = null!;

        public bool? UserOldEnough { get; set; }
    }
}

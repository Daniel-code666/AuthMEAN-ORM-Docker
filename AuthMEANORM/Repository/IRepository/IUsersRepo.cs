using AuthMEANORM.Models.UsersModel;

namespace AuthMEANORM.Repository.IRepository
{
    public interface IUsersRepo
    {
        Task<ICollection<Users>> GetUsers();

        Task<Users?> GetUser(string id_user);

        Task<bool> CreateUser(Users user);

        Task<bool> UpdateUser(Users user);

        Task<bool> DeleteUserById(string id_user);

        Task<bool> CheckUserExist(string id_user);

        Task<Users> Register(Users user);

        Task<Users> Login(Users user);
    }
}

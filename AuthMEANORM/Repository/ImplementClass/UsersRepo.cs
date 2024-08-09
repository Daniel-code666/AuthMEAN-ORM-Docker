using AuthMEANORM.Context;
using AuthMEANORM.Models.UsersModel;
using AuthMEANORM.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthMEANORM.Repository.ImplementClass
{
    public class UsersRepo : IUsersRepo
    {
        private readonly AppDbContext _db;

        public UsersRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CreateUser(Users user)
        {
            try
            {
                await _db.Users.AddAsync(user);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteUserById(string id_user)
        {
            try
            {
                var objId = GetObjectId(id_user);
                var userToDelete = objId.HasValue ? await _db.Users.FirstOrDefaultAsync(u => u.Id == objId.Value) : null;

                _db.Users.Remove(userToDelete!);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        public async Task<Users?> GetUser(string id_user)
        {
            try
            {
                var objId = GetObjectId(id_user);

                return await _db.Users.FirstOrDefaultAsync(u => u.Id == objId);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Users();
            }
        }

        public async Task<ICollection<Users>> GetUsers()
        {
            try
            {
                return await _db.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Users>();
            }
        }

        public async Task<bool> UpdateUser(Users user)
        {
            try
            {
                _db.Users.Update(user);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> CheckUserExist(string id_user)
        {
            try
            {
                var objId = GetObjectId(id_user);

                return await _db.Users.AnyAsync(u => u.Id == objId);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        public async Task<Users> Login(Users user)
        {
            throw new NotImplementedException();
        }

        public async Task<Users> Register(Users user)
        {
            throw new NotImplementedException();
        }

        private ObjectId? GetObjectId(string id_user)
        {
            return ObjectId.TryParse(id_user, out ObjectId objId) ? objId : (ObjectId?)null;
        }
    }
}

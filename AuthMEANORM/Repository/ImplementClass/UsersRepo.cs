using AuthMEANORM.Context;
using AuthMEANORM.Models.UsersModel;
using AuthMEANORM.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;
using System.Security.Claims;
using XSystem.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace AuthMEANORM.Repository.ImplementClass
{
    public class UsersRepo(AppDbContext db, IConfiguration conf) : IUsersRepo
    {
        private readonly AppDbContext _db = db;
        private readonly string secretKey = conf.GetValue<string>("ApiSettings:key")!;

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

        public async Task<bool> CheckUserExistByEmail(string email)
        {
            try
            {
                return await _db.Users.AnyAsync(u => u.Email == email);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Users?> GetUserByEmail(string email)
        {
            try
            {
                return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Users?> Login(LoginUser user)
        {
            try
            {
                var found_user = await CheckUserLogin(user);

                if (found_user == null) return null;

                string token_string = TknHandler(found_user);

                return new Users
                {
                    Email = found_user.Email,
                    Token = token_string,
                    UserRoles = found_user.UserRoles,
                    IsActive = found_user.IsActive,
                    UserName = found_user.UserName,
                };
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Users();
            }
        }

        public async Task<Users> Register(Users user)
        {
            var enc_pass = GetMd5(user.UserPass);

            if (enc_pass != "")
            {
                user.UserPass = enc_pass;

                await _db.Users.AddAsync(user);

                await _db.SaveChangesAsync();

                user.UserPass = enc_pass;

                return user;
            }
            else
            {
                return new Users();
            }
        }

        private async Task<Users?> CheckUserLogin(LoginUser user)
        {
            var encryptedPass = GetMd5(user.UserPass);

            var found_user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower() && u.UserPass == encryptedPass);

            return found_user != null ? found_user : null;
        }

        private string TknHandler(Users found_user)
        {
            var tknHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, found_user.Email)
            };

            if (found_user.UserRoles != null)
            {
                foreach (var role in found_user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var createdToken = tknHandler.CreateToken(tokenDesc);
            var tokenString = tknHandler.WriteToken(createdToken);

            return tokenString;
        }

        private static string GetMd5(string userPass)
        {
            try
            {
                string resp = "";

                MD5CryptoServiceProvider x = new();

                byte[] data = System.Text.Encoding.UTF8.GetBytes(userPass);

                data = x.ComputeHash(data);

                for (int i = 0; i < data.Length; i++)
                {
                    resp += data[i].ToString("x2").ToLower();
                }

                return resp;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return "";
            }
        }

        private ObjectId? GetObjectId(string id_user)
        {
            return ObjectId.TryParse(id_user, out ObjectId objId) ? objId : (ObjectId?)null;
        }
    }
}

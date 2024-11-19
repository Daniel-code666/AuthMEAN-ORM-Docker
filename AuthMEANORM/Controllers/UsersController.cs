using AuthMEANORM.Models.UsersModel;
using AuthMEANORM.Repository.ImplementClass;
using AuthMEANORM.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AuthMEANORM.Utils;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using static AuthMEANORM.Utils.UpdateUserModels;
using Microsoft.AspNetCore.Authorization;

namespace AuthMEANORM.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController(IUsersRepo usersRepo) : ControllerBase
    {
        private readonly IUsersRepo _usersRepo = usersRepo;

        [HttpGet("get_users")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return Ok(new ApiResponse<IEnumerable<Users>>("OK", await _usersRepo.GetUsers()));
            } 
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>(ex.Message, []));
            }
        }

        [HttpGet("get_user")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> GetUser([FromQuery] string id_user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id_user))
                {
                    return StatusCode(400, new ApiResponse<string>("Bad request", ""));
                }

                if(!await _usersRepo.CheckUserExist(id_user))
                {
                    return StatusCode(400, new ApiResponse<string>("User not found", ""));
                }

                return Ok(new ApiResponse<Users>("OK", await _usersRepo.GetUser(id_user)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message, ""));
            }
        }

        [HttpPost("create_user")]
        public async Task<IActionResult> CreateUsers([FromBody] Users user)
        {
            try
            {
                if(!ModelState.IsValid || user == null)
                {
                    return StatusCode(400, new ApiResponse<string>("Bad request", ""));
                }

                if (!await _usersRepo.CreateUser(user))
                {
                    return StatusCode(500, new ApiResponse<string>("Error", ""));
                }

                var response = new
                {
                    StatusCode = "Ok",
                    Message = "User created"
                };

                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message, ""));
            }
        }

        [AllowAnonymous]
        [HttpPost("register_user")]
        public async Task<IActionResult> RegisterUser([FromBody] Users user)
        {
            try
            {
                if (!ModelState.IsValid || user == null)
                {
                    return StatusCode(400, new ApiResponse<string>("Bad request", ""));
                }

                await _usersRepo.Register(user);

                return Ok(new ApiResponse<string>("Usuario creado", ""));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message, ""));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser user)
        {
            try
            {
                if (!ModelState.IsValid || user == null)
                {
                    return StatusCode(400, new ApiResponse<string>("Bad request", ""));
                }

                var loggedUser = await _usersRepo.Login(user);

                if (loggedUser == null)
                {
                    return StatusCode(404, new ApiResponse<string>("User not found", ""));
                }

                return Ok(new ApiResponse<Users>("OK", loggedUser));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message, ""));
            }
        }

        [HttpDelete("delete_user")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id_user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id_user))
                {
                    return StatusCode(400, new ApiResponse<string>("Bad request", ""));
                }

                if (!await _usersRepo.CheckUserExist(id_user))
                {
                    return StatusCode(404, new ApiResponse<string>("User not found", ""));
                }

                if(!await _usersRepo.DeleteUserById(id_user))
                {
                    return StatusCode(500, new ApiResponse<string>("Error", ""));
                }

                return Ok(new ApiResponse<string>("User deleted", ""));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message, ""));
            }
        }

        [HttpPatch("update_user")]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                // Validar si el cuerpo de la solicitud es nulo
                // Validar el body
                if (request == null || string.IsNullOrWhiteSpace(request.Identifier) || request.Data == null || !request.Data.Any())
                {
                    return StatusCode(400, new ApiResponse<string>("Invalid input", ""));
                }

                // Verificar si el usuario existe
                if (!await _usersRepo.CheckUserExistByEmail(request.Identifier))
                {
                    return StatusCode(404, new ApiResponse<string>("User not found", ""));
                }

                // Obtener el usuario por correo electrónico
                var user = await _usersRepo.GetUserByEmail(request.Identifier);
                if (user == null)
                {
                    return StatusCode(404, new ApiResponse<string>("User not found", ""));
                }

                // Crear un JsonPatchDocument a partir del `data` recibido
                var patchDoc = new JsonPatchDocument<Users>();
                foreach (var operation in request.Data)
                {
                    patchDoc.Operations.Add(new Operation<Users>
                    {
                        op = operation.Op,
                        path = operation.Path,
                        value = operation.Value
                    });
                }

                // Aplicar las operaciones al usuario
                patchDoc.ApplyTo(user, ModelState);

                // Validar el modelo después de aplicar los cambios
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, new ApiResponse<string>("Invalid patch operation", ""));
                }

                // Actualizar el usuario en el repositorio
                if (!await _usersRepo.UpdateUser(user))
                {
                    return StatusCode(500, new ApiResponse<string>("Error updating user", ""));
                }

                return Ok(new ApiResponse<Users>("User updated", user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(ex.Message,""));
            }
        }
    }
}

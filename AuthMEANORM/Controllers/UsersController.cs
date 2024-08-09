using AuthMEANORM.Models.UsersModel;
using AuthMEANORM.Repository.ImplementClass;
using AuthMEANORM.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace AuthMEANORM.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController(IUsersRepo usersRepo) : ControllerBase
    {
        private readonly IUsersRepo _usersRepo = usersRepo;

        [HttpGet("get_users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var response = new
                {
                    StatusCode = "OK",
                    users = await _usersRepo.GetUsers()
                };

                return Ok(response);
            } 
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    StatusCode = "ERROR",
                    Message = "An error occurred while processing the request."
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("create_user")]
        public async Task<IActionResult> CreateUsers([FromBody] Users user)
        {
            try
            {
                if(!ModelState.IsValid || user == null)
                {
                    var errorResponse = new
                    {
                        StatusCode = 400,
                        Message = "BadRequest"
                    };

                    return StatusCode(400, errorResponse);
                }

                if (!await _usersRepo.CreateUser(user))
                {
                    var errorResponse = new
                    {
                        StatusCode = 500,
                        Message = "ERROR"
                    };

                    return StatusCode(500, errorResponse);
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
                var errorResponse = new
                {
                    StatusCode = "ERROR",
                    Message = "An error occurred while processing the request."
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("delete_user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id_user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id_user))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "BadRequest"
                    });
                }

                if (!await _usersRepo.CheckUserExist(id_user))
                {
                    var errorResponse = new
                    {
                        StatusCode = 400,
                        Message = "User not exists"
                    };

                    return StatusCode(400, errorResponse);
                }

                if(!await _usersRepo.DeleteUserById(id_user))
                {
                    var errorResponse = new
                    {
                        StatusCode = 500,
                        Message = "ERROR"
                    };

                    return StatusCode(500, errorResponse);
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "User deleted"
                });
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    StatusCode = "ERROR",
                    Message = "An error occurred while processing the request."
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpPatch("update_user")]
        public async Task<IActionResult> UpdateUser([FromQuery] string id_user, [FromBody] JsonPatchDocument<Users> patchDoc)
        {
            try
            {
                if (patchDoc == null || string.IsNullOrWhiteSpace(id_user))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Invalid input."
                    });
                }

                if (!await _usersRepo.CheckUserExist(id_user))
                {
                    var errorResponse = new
                    {
                        StatusCode = 400,
                        Message = "User doesn't exists"
                    };

                    return StatusCode(400, errorResponse);
                }

                var user = await _usersRepo.GetUser(id_user);
                patchDoc.ApplyTo(user, ModelState);

                if (!await _usersRepo.UpdateUser(user!))
                {
                    return StatusCode(500, new
                    {
                        StatusCode = 500,
                        Message = "Failed to update user."
                    });
                }

                return StatusCode(200, new { updt_cat = user });
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    StatusCode = "ERROR",
                    Message = "An error occurred while processing the request."
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gretly.Utils;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Gretly.Dto;
using FaunaDB.Errors;
using Gretly.Constants;
using Gretly.Services;
using System.Collections.Generic;
using Gretly.Models;

namespace Gretly.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        private string GetAuthorizationHeader()
        {
            return Request.Headers["Authorization"].ToString();
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.User>), 200)]
        [ProducesResponseType(typeof(IReadOnlyList<QueryError>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var users = await userService.GetUsers();
                return Ok(users);
            }
            catch (FaunaException ex)
            {
                if (ex.StatusCode == 400)
                {
                    return BadRequest(ex.Errors);
                }
                else
                {
                    return StatusCode(ex.StatusCode);
                }
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.User>), 200)]
        [ProducesResponseType(typeof(IReadOnlyList<QueryError>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetUserById(string id)
        {
            try
            {
                var user = await userService.GetUserById(id);
                return Ok(user);
            }
            catch (FaunaException ex)
            {
                if (ex.StatusCode == 400)
                {
                    return BadRequest(ex.Errors);
                }
                else
                {
                    return StatusCode(ex.StatusCode, ex.ToString());
                }
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Models.User), 200)]
        [ProducesResponseType(typeof(IReadOnlyList<QueryError>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateUser(Models.User user)
        {
            try
            {
                var newUser = await userService.CreateUser(user);
                return Ok(newUser);
            }
            catch (FaunaException ex)
            {
                if (ex.StatusCode == 400)
                {
                    return BadRequest(ex.Errors);
                }
                else
                {
                    return StatusCode(ex.StatusCode);
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<Models.User>), 200)]
        [ProducesResponseType(typeof(IReadOnlyList<QueryError>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UserDto data)
        {
            try
            {
                var updatedUser = await userService.UpdateUser(id, data);
                if (updatedUser == null)
                {
                    return BadRequest(ErrorReasons.USER_NOT_EXISTS);
                }
                return Ok(updatedUser);
            }
            catch (FaunaException ex)
            {
                if (ex.StatusCode == 400)
                {
                    return BadRequest(ex.Errors);
                }
                else
                {
                    return StatusCode(ex.StatusCode, ex.ToString());
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiSuccessResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                var token = Helpers.DecodeToken(GetAuthorizationHeader());
                var currentUser = await userService.FindUserByToken(token);
                if (currentUser.Id == id)
                {
                    return BadRequest(new ApiError(ErrorReasons.CANT_DELETE_CURRENT_USER, "Current user cannot be deleted"));
                }
                try
                {
                    await FirebaseClient.GetClient().DeleteUserAsync(token.UserId);
                    userService.DeleteUser(token.UserId);
                    return Ok(new ApiSuccessResponse("Deleted successfully"));
                }
                catch (FaunaException ex)
                {
                    if (ex.StatusCode == 400)
                    {
                        return BadRequest(ex.Errors);
                    }
                    else
                    {
                        return StatusCode(ex.StatusCode);
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
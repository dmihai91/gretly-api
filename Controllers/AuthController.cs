using System.Threading.Tasks;
using GretlyStudio.Utils;
using GretlyStudio.Models;
using GretlyStudio.Dto;
using Microsoft.AspNetCore.Mvc;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using FaunaDB.Errors;
using GretlyStudio.Constants;

using GretlyStudio.Services;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;

namespace GretlyStudio.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly IUserService userService;
        private readonly IConfiguration config;

        public AuthController(IWebHostEnvironment hostingEnv, IUserService userService, IConfiguration config)
        {
            env = hostingEnv;
            this.userService = userService;
            this.config = config; 
        }

        private async Task<OkObjectResult> AuthOkResponse(FirebaseAuthLink firebaseResponse, Models.User user)
        {
            var data = Helpers.GenerateRefreshToken(firebaseResponse, env);
            PropertyInfo cookieOptionsProp = data.GetType().GetProperty("cookieOptions");
            PropertyInfo jsonResultProp = data.GetType().GetProperty("jsonResult");
            CookieOptions cookieOptions = (CookieOptions)cookieOptionsProp.GetValue(data);

            JsonResult jsonResult = (JsonResult)jsonResultProp.GetValue(data);
            Response.Cookies.Append("refresh_token", firebaseResponse.RefreshToken, cookieOptions);
            // set lastLoggedIn field to Now
            await userService.UpdateUser(user.Id, new KeyValuePair<string, DateTime>("LastLoggedIn", DateTime.Now));
            return Ok(jsonResult.Value);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Login([Description("User credentials(username or email, password)")][FromBody] LoginInfo loginInfo)
        {
            if (loginInfo.Username == null || loginInfo.Password == null)
            {
                return BadRequest(new ApiError(ErrorReasons.INVALID_CREDENTIALS, "Invalid email or password"));
            }
            try
            {
                var user = await userService.FindUser(loginInfo.Username);
                if (user != null)
                {
                    try
                    {
                        var firebaseResponse = await FirebaseClient.GetClient().SignInWithEmailAndPasswordAsync(user.Email, loginInfo.Password);
                        // set lastLoggedIn field to Now
                        await userService.UpdateUser<System.DateTime>(user.Id, new KeyValuePair<string, DateTime>("LastLoggedIn", System.DateTime.Now));
                        return await AuthOkResponse(firebaseResponse, user);
                    }
                    catch (FirebaseAuthException ex)
                    {
                        if (ex.Reason == AuthErrorReason.InvalidEmailAddress || ex.Reason == AuthErrorReason.WrongPassword || ex.Reason == AuthErrorReason.UnknownEmailAddress)
                        {
                            var apiError = new ApiError(ErrorReasons.INVALID_CREDENTIALS, "Invalid username or password", ex.ResponseData);
                            Log.Information(JsonConvert.SerializeObject(apiError));
                            return BadRequest(apiError.getErrorWithoutResponseData());
                        }
                        else if (ex.Reason == AuthErrorReason.UserDisabled)
                        {
                            var apiError = new ApiError(ErrorReasons.USER_DISABLED, "User account has been disabled by an administrator", ex.ResponseData);
                            Log.Information(JsonConvert.SerializeObject(apiError));
                            return BadRequest(apiError.getErrorWithoutResponseData());
                        }
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    var apiError = new ApiError(ErrorReasons.INVALID_CREDENTIALS, "Invalid username or password");
                    Log.Information(JsonConvert.SerializeObject(apiError));
                    return BadRequest(apiError.getErrorWithoutResponseData());

                }
            }
            catch (FaunaException ex)
            {
                return StatusCode(ex.StatusCode, ex.ToString());
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Models.User), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(500)]

        public async Task<ActionResult> Register([FromBody] RegistrationInfo registrationInfo)
        {
            try
            {
                var response = await FirebaseClient.GetClient().CreateUserWithEmailAndPasswordAsync(registrationInfo.Email, registrationInfo.Password);
                try
                {
                    var newUser = await userService.CreateUser(new Models.User(response.User.LocalId, registrationInfo.Username, registrationInfo.Email, registrationInfo.Name));
                    return Ok(Helpers.FormatData(newUser));
                }
                catch (FaunaException ex)
                {
                    if (ex.StatusCode == 400)
                    {
                        await FirebaseClient.GetClient().DeleteUserAsync(response.FirebaseToken);
                        if (ex.Errors[0].Code == "instance not unique")
                        {
                            return BadRequest(new ApiError(ErrorReasons.USER_ALREADY_EXISTS, "User already exists"));
                        }
                        else
                        {
                            return BadRequest(ex.Errors);
                        }
                    }
                    else
                    {
                        return StatusCode(ex.StatusCode, ex.ToString());
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                switch (ex.Reason)
                {
                    case AuthErrorReason.InvalidEmailAddress:
                        return BadRequest(new ApiError(ErrorReasons.INVALID_EMAIL_ADDRESS, "Invalid email address", ex.ResponseData));
                    case AuthErrorReason.EmailExists:
                        return BadRequest(new ApiError(ErrorReasons.EMAIL_ALREADY_EXISTS, "Email already exists", ex.ResponseData));
                    case AuthErrorReason.MissingEmail:
                        return BadRequest(new ApiError(ErrorReasons.NO_EMAIL_PROVIDED, "Email was not provided", ex.ResponseData));
                    case AuthErrorReason.MissingPassword:
                        return BadRequest(new ApiError(ErrorReasons.NO_PASSWORD_PROVIDED, "Password was not provided", ex.ResponseData));
                    case AuthErrorReason.WeakPassword:
                        return BadRequest(new ApiError(ErrorReasons.WEAK_PASSWORD, "Password should be at leasts 6 characters long", ex.ResponseData));
                    default:
                        return BadRequest(ex.ResponseData);
                }
            }
        }

        [HttpPost("authWithFacebook")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> AuthWithFacebook([FromBody] FBData fbData)
        {
            if (fbData.AccessToken == null)
            {
                return BadRequest(new ApiError(ErrorReasons.NO_ACCESS_TOKEN_PROVIDED, "No access token was provided"));
            }
            try
            {
                var firebaseResponse = await FirebaseClient.GetClient().SignInWithOAuthAsync(FirebaseAuthType.Facebook, fbData.AccessToken);
                // check if user already exists
                var response = await userService.FindUserByEmail(fbData.Email);
                if (response == null)
                {
                    try
                    {
                        var newUser = await userService.CreateUser(new Models.User(fbData));
                        try
                        {
                            string password = Helpers.GeneratePassword();
                            await firebaseResponse.LinkToAsync(fbData.Email, password);
                            if (!firebaseResponse.User.IsEmailVerified)
                            {
                                await FirebaseClient.GetClient().SendEmailVerificationAsync(firebaseResponse.FirebaseToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            return StatusCode(500);
                        }
                        return await AuthOkResponse(firebaseResponse, newUser);
                    }
                    catch (FaunaException ex)
                    {
                        if (ex.StatusCode == 400)
                        {
                            if (ex.Errors[0].Code == "instance not unique")
                            {
                                return BadRequest(new ApiError(ErrorReasons.USER_ALREADY_EXISTS, "User already exists"));
                            }
                        }
                        return StatusCode(ex.StatusCode, new
                        {
                            error = ex.Message
                        });
                    }
                }
                return await AuthOkResponse(firebaseResponse, response);
            }
            catch (FirebaseAuthException ex)
            {
                var apiError = new ApiError(ErrorReasons.INVALID_ACCESS_TOKEN, "Invalid access token", ex.ResponseData);
                Log.Information(JsonConvert.SerializeObject(apiError));
                return BadRequest(apiError.getErrorWithoutResponseData());
            }
        }

        [HttpPost("authWithGoogle")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> AuthWithGoogle([FromBody] GoogleData googleData)
        {
            if (googleData.AccessToken == null)
            {
                return BadRequest(new ApiError(ErrorReasons.NO_ACCESS_TOKEN_PROVIDED, "No access token was provided"));
            }
            try
            {
                var firebaseResponse = await FirebaseClient.GetClient().SignInWithOAuthAsync(FirebaseAuthType.Google, googleData.AccessToken);
                // check if user already exists
                var response = await userService.FindUserByEmail(googleData.Email);
                if (response == null)
                {
                    try
                    {
                        var newUser = await userService.CreateUser(new Models.User(googleData));
                        try
                        {
                            string password = Helpers.GeneratePassword();
                            await firebaseResponse.LinkToAsync(googleData.Email, password);
                            if (!firebaseResponse.User.IsEmailVerified)
                            {
                                await FirebaseClient.GetClient().SendEmailVerificationAsync(firebaseResponse.FirebaseToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            return StatusCode(500);
                        }
                        return await AuthOkResponse(firebaseResponse, newUser);
                    }
                    catch (FaunaException ex)
                    {
                        if (ex.StatusCode == 400)
                        {
                            if (ex.Errors[0].Code == "instance not unique")
                            {
                                return BadRequest(new ApiError(ErrorReasons.USER_ALREADY_EXISTS, "User already exists"));
                            }
                        }
                        return StatusCode(ex.StatusCode, new
                        {
                            error = ex.Message
                        });
                    }
                }
                return await AuthOkResponse(firebaseResponse, response);
            }
            catch (FirebaseAuthException ex)
            {
                var apiError = new ApiError(ErrorReasons.INVALID_ACCESS_TOKEN, "Invalid access token", ex.ResponseData);
                Log.Information(JsonConvert.SerializeObject(apiError));
                return BadRequest(apiError.getErrorWithoutResponseData());
            }
        }


        [HttpGet("getLoggedUser")]
        [Authorize]
        [ProducesResponseType(typeof(Models.User), 200)]
        [ProducesResponseType(typeof(FaunaException), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetUserDetails()
        {
            try
            {
                var token = Helpers.DecodeToken(Helpers.GetAuthorizationHeader(Request));
                try
                {
                    var user = await userService.FindUserByToken(token);
                    return Ok(Helpers.FormatData(user));
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

        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> RefreshToken()
        {
            HttpClient client = new HttpClient();
            var refreshToken = Request.Cookies["refresh_token"];
            if (refreshToken == null)
            {
                return Unauthorized(new ApiError(ErrorReasons.NO_REFRESH_TOKEN, "No refresh token provided"));
            }

            var uri = new Uri(config["OAuthRefreshTokenServerUrl"] + FirebaseClient.GetApiKey());

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            IList<KeyValuePair<string, string>> formContent = new List<KeyValuePair<string, string>> {
                { new KeyValuePair<string, string>("grant_type", "refresh_token") },
                { new KeyValuePair<string, string>("refresh_token", refreshToken) },
            };
            request.Content = new FormUrlEncodedContent(formContent);
            var response = await client.SendAsync(request);
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.ReasonPhrase == "Bad Request")
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(result);
                if (errorResponse.Error.Message == ErrorReasons.INVALID_REFRESH_TOKEN)
                {
                    return BadRequest(new ApiError(ErrorReasons.INVALID_REFRESH_TOKEN, "Invalid refresh token"));
                }
                return BadRequest(errorResponse);
            }
            else
            {
                var responseContent = JsonConvert.DeserializeObject<RefreshTokenResponse>(result);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true
                };

                if (!env.IsDevelopment())
                {
                    cookieOptions.Secure = true;
                }
                Response.Cookies.Append("refresh_token", responseContent.RefreshToken, cookieOptions);
                var jsonResult = new JsonResult(new
                {
                    accessToken = responseContent.AccessToken,
                    refreshToken = responseContent.RefreshToken,
                    expiresIn = responseContent.ExpiresIn,
                    sessionId = Guid.NewGuid()
                });
                return jsonResult;
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult Logout()
        {
            HttpClient client = new HttpClient();
            Response.Cookies.Delete("refresh_token");
            var uri = new Uri(config["OAuthRevokeTokenServerUrl"]);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var authHeader = Request.Headers["Authorization"].ToString();
            var jwt = authHeader["Bearer ".Length..].Trim();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            try
            {
                client.SendAsync(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
            return Ok();
        }
    }
}


using System.IdentityModel.Tokens.Jwt;
using GretlyStudio.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FireSharp.Extensions;

namespace GretlyStudio.Utils
{
    public class Helpers
    {
        public static FirebaseDto DecodeToken(string authHeader)
        {
            var jwt = authHeader["Bearer ".Length..].Trim();
            var handler = new JwtSecurityTokenHandler();
            var token = (FirebaseDto)handler.ReadJwtToken(jwt);
            return token;
        }

        public static string GetAuthorizationHeader(HttpRequest Request)
        {
            return Request.Headers["Authorization"].ToString();
        }

        public static object GenerateRefreshToken(Firebase.Auth.FirebaseAuthLink firebaseResponse, IWebHostEnvironment env)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMonths(1)
            };

            if (!env.IsDevelopment())
            {
                cookieOptions.Secure = true;
            }
            //cookies.Append("refreshToken", response.RefreshToken, cookieOptions);
            var jsonResult = new JsonResult(new
            {
                accessToken = firebaseResponse.FirebaseToken,
                expiresIn = firebaseResponse.ExpiresIn,
                sessionId = Guid.NewGuid()
            });
            return new
            {
                jsonResult,
                cookieOptions
            };
        }

        public static T FormatData<T>(T data)
        {
            var json = data.ToJson();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string GeneratePassword()
        {
            var generator = new PasswordGenerator(minimumLengthPassword: 8,
                            maximumLengthPassword: 15,
                            minimumUpperCaseChars: 2,
                            minimumNumericChars: 3,
                            minimumSpecialChars: 2);
            return generator.Generate();
        }

        /*public static Task SendActivationEmail(string to, string activationLink)
        {
            var client = new SendGridClient(apiKey);
            string html = File.ReadAllText(@"../Assets/ActivationEmail.html");
        }*/
    }
}

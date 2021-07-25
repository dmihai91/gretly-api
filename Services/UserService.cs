using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Errors;
using FaunaDB.Types;
using Gretly.Constants;
using Gretly.Dto;
using Gretly.Models;
using Gretly.Utils;

namespace Gretly.Services
{
    public class UserService : IUserService
    {
        public async Task<User> FindUserByEmail(string email)
        {
            try
            {
                return await DBHelpers.FindTermInIndex<User>(DBIndexes.UNIQUE_USER_EMAIL, email);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByUsername(string username)
        {
            try
            {
                return await DBHelpers.FindTermInIndex<User>(DBIndexes.UNIQUE_USER_USERNAME, username);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        // find username by username or email
        public async Task<User> FindUser(string username)
        {
            try
            {
                var userByUsername = await this.FindUserByUsername(username);
                var userByEmail = await this.FindUserByEmail(username);
                return userByUsername != null ? userByUsername : (userByEmail != null ? userByEmail : null);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByToken(FirebaseDto token)
        {
            try
            {
                if (token.Email != null)
                {
                    return await this.FindUserByEmail(token.Email);
                }
                else
                {
                    var value = await FaunaDbClient.GetDocumentByIndex(DBIndexes.UNIQUE_USER_FBASE_USER_ID, token.UserId);
                    var result = value.At("data").To<FaunaResultDto>().Value;
                    var user = result.Data.To<User>().Value;
                    user.Id = result.Ref.Id;
                    return user;
                }
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> CreateUser(User user)
        {
            try
            {
                var value = await FaunaDbClient.CreateDocument(DBCollections.USER, user);
                var result = Decoder.Decode<FaunaResultDto>(value);
                var newUser = Decoder.Decode<User>(value.At("data"));
                newUser.Id = result.Ref.Id;
                return newUser;
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                return await DBHelpers.GetAllDocumentsFromCollection<User>(DBCollections.USER);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserById(string id)
        {
            try
            {
                var value = await FaunaDbClient.GetDocument(DBCollections.USER, id);
                var result = Decoder.Decode<FaunaResultDto>(value);
                var user = Decoder.Decode<User>(value.At("data"));
                user.Id = result.Ref.Id;
                return user;
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> UpdateUser(string id, UserDto user)
        {

            try
            {
                var value = await FaunaDbClient.GetDocument(DBCollections.USER, id);
                var userById = Decoder.Decode<User>(value.At("data"));
                if (user.Id != userById.Id)
                {
                    return null;
                }

                value = await FaunaDbClient.UpdateDocument(DBCollections.USER, id, new User(user));
                var updatedUser = Decoder.Decode<User>(value.At("data"));
                return updatedUser;
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async Task<User> UpdateUser<T>(string id, KeyValuePair<string, T> updateData)
        {
            try
            {
                var value = await FaunaDbClient.GetDocument(DBCollections.USER, id);
                var user = Decoder.Decode<User>(value.At("data"));
                user.SetProperty<T>(updateData.Key, updateData.Value);
                await FaunaDbClient.UpdateDocument(DBCollections.USER, id, user);
                return user;
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }

        public async void DeleteUser(string id)
        {
            try
            {
                await FaunaDbClient.DeleteDocument(DBCollections.USER, id);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }
    }
}
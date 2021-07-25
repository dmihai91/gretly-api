using System.Collections.Generic;
using System.Threading.Tasks;
using Gretly.Dto;
using Gretly.Models;

namespace Gretly.Services
{
    public interface IUserService
    {
        Task<User> FindUserByEmail(string email);
        Task<User> FindUserByUsername(string username);
        Task<User> FindUser(string username);
        Task<User> FindUserByToken(FirebaseDto token);
        Task<User> CreateUser(User user);
        Task<List<User>> GetUsers();
        Task<User> GetUserById(string id);
        Task<User> UpdateUser(string id, UserDto user);
        Task<User> UpdateUser<T>(string id, KeyValuePair<string, T> updateData);
        void DeleteUser(string id);
    }
}
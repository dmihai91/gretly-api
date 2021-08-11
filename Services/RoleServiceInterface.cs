using System.Collections.Generic;
using System.Threading.Tasks;
using GretlyStudio.Models;

namespace GretlyStudio.Services
{
    public interface IRoleService
    {
        // Get all the roles from the db
        Task<List<Role>> GetRoles();
    }
}
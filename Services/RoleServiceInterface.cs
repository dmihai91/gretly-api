using System.Collections.Generic;
using System.Threading.Tasks;
using Gretly.Models;

namespace Gretly.Services
{
    public interface IRoleService
    {
        // Get all the roles from the db
        Task<List<Role>> GetRoles();
    }
}
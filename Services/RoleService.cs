using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Errors;
using Gretly.Constants;
using Gretly.Models;
using Gretly.Utils;

namespace Gretly.Services
{
    public class RoleService : IRoleService
    {
        public async Task<List<Role>> GetRoles()
        {
            try
            {
                return await DBHelpers.GetAllDocumentsFromCollection<Role>(DBCollections.ROLE);
            }
            catch (FaunaException ex)
            {
                throw ex;
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using FaunaDB.Errors;
using GretlyStudio.Constants;
using GretlyStudio.Models;
using GretlyStudio.Utils;

namespace GretlyStudio.Services
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
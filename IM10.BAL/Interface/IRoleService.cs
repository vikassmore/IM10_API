using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for role related operations
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Method is used to get role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        RoleModel GetRoleById(long roleId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all roles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<RoleModel> GetAllRoles(ref ErrorResponseModel errorResponseModel);

    }
}

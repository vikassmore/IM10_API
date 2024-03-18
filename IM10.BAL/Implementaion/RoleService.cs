using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{

    /// <summary>
    /// This is implementation for the roles operations 
    /// </summary>
    public class RoleService : IRoleService
    {
        IM10DbContext context;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>

        public RoleService(IM10DbContext _context)
        {
            context = _context;
        }


        /// <summary>
        /// Method to get roles by roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public RoleModel GetRoleById(long roleId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var roleEntity=context.Roles.FirstOrDefault(x=>x.RoleId==roleId && x.IsDeleted==false);
            if (roleEntity==null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            return new RoleModel
            {
                RoleId = roleEntity.RoleId,
                Name = roleEntity.Name,
                Description = roleEntity.Description,
            };
            
        }



        /// <summary>
        /// Method to get all roles 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<RoleModel> GetAllRoles(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var roleList=new List<RoleModel>();
            var roleEntity=context.Roles.Where(x=>x.IsDeleted==false).ToList();
            if (roleEntity.Count == 0)
            {
                errorResponseModel.StatusCode= HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            roleEntity.ForEach(item =>
            {
                roleList.Add(new RoleModel
                {
                    RoleId=item.RoleId,
                    Name = item.Name,
                    Description = item.Description,
                });
            });
            return roleList;
        }


    }
}

using IM10.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for user related operations
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Method is used to get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserModel GetUserById(long userId,ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<UserModel1> GetAllUser(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get Other user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<UserModel1>GetOtherUser(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string AddUser (UserModel user,ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to Edit user
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        string EditUser(UserModel users,ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string DeleteUser(long userId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to Forget password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        string ForgetPassword(string email,ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to Change password
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        string ChangePassword(changepasswordModel userModel, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add mobile user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMobileUser(MobileUserRegisterModel model, ref ErrorResponseModel errorResponseModel);

        string ActivateUser(ActiveUserModel model, ref ErrorResponseModel errorResponseModel);


    }
}

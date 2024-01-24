using IM10.Models;
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
    public interface IUserPlayerService
    {
        /// <summary>
        /// Method is used to get userplayer by id
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <returns></returns>
        UserPlayerModel GetUserPlayerById(long userplayerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get userplayerBy UserplayerId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        UserPlayerModel2 GetuserplayerByUserplayerId(long userplayerId,ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get all userplayer
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<UserPlayerModel> GetAllUserPlayer(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all userplayer
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<UserPlayerModel3> GetAllUserPlayerdetailsbycommaseparate(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add userplayer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddUserPlayer(UserPlayerModel1 model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to Edit userplayer
        /// </summary>
        /// <param name="playerModel"></param>
        /// <returns></returns>
        string EditUserPlayer(UserPlayerMappingModel playerModel, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete userplayer
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <returns></returns>
        string DeleteUserPlayer(long userplayerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get player by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
       List< UserPlayerModel> GetPlayerByUserId(long userId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get player by roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<UserPlayerModel> GetPlayerByRoleId(long roleId,  ref ErrorResponseModel errorResponseModel);


    }
}

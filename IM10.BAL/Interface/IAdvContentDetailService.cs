using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for AdvContentDetail related operations
    /// </summary>

    public interface IAdvContentDetailService
    {
        /// <summary>
        /// Method is used to get AdvContentDetail by id
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <returns></returns>
        AdvContentDetailsModel GetAdvContentDetailById(long AdvertiseContentId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get AdvContentDetail by id
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
       List< AdvContentDetailsModel> GetAdvContentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get approvedAdvContentDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<AdvContentDetailsModel> GetApprovedAdvContentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);




        /// <summary>
        /// Method is used to get all AdvContentDetail
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<AdvContentDetailsModel> GetAllAdvContentDetail(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to add/edit AdvContentDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddAdvContentDetail(AdvContentDetailsModel1 model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete AdvContentDetail
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <returns></returns>
        string DeleteAdvContentDetail(long AdvertiseContentId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get AdvContentDetail by id
        /// </summary>
        /// <param name="advertisecontentId"></param>
        /// <returns></returns>
        string ApproveAdvContentDetail(long advertisecontentId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to denied AdvContentDetail 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string DeniedAdvContentDetail(AdvContentComment model, ref ErrorResponseModel errorResponseModel);


    }
}

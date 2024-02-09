using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for contentDetails related operations
    /// </summary>
    public interface IContentDetailService
    {
        /// <summary>
        /// Method is used to get contentDetails by id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        ContentDetailModel GetContentDetailById(long contentId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all contentDetails
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<ContentDetailModel> GetAllContentDetail(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to add contentDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddContentDetail(ContentDetailModel1 model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to Edit contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <returns></returns>
        string EditContentDetail(ContentDetailModel1 contentModel, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete contentDetails
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        string DeleteContentDetail(long contentId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get contentdetails by id
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        List<ContentDetailModel> GetContentdetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get approvedcontentdetails by id
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        List<ContentDetailModel> GetApprovedContentdetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to approve contentdetails by id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        Task <NotificationModel> ApproveContentDetail(long contentId);



        /// <summary>
        /// Method is used to denied contentdetails 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string DeniedContentDetail(CommentModel model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to approve contentdetails
        /// </summary>
        /// <param name="model1"></param>
        /// <returns></returns>
        string ContentDetailUpdateByContentLog(ContentModel model1,ref ErrorResponseModel errorResponseModel);

    }
}

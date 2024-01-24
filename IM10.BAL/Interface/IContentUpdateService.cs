using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for contentUpdate related operations
    /// </summary>
    public interface IContentUpdateService
    {
        /// <summary>
        /// Method is used to get contentUpdate by id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
       List< ContentUpdateModel> GetContentUpdate(long contentId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contentUpdate by id
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ContentUpdateModel> GetContentUpdateByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);



        /// <summary>
        /// Method is used to get contentUpdate by id
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
       List< ContentUpdateModel> GetContentUpdateforQA(long playerId, ref ErrorResponseModel errorResponseModel);



        /// <summary>
        /// Method is used to denied contentupdatedetails 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string DeniedContentUpdateDetail(ContentUpdateComment model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all contentUpdate
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<ContentUpdateModel> GetAllContentUpdate(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all contentTitles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<ContentTitleModel> GetAllContentTitles(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to add contentUpdate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddContentUpdate(ContentUpdateModel model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete contentUpdate
        /// </summary>
        /// <param name="contentLogId"></param>
        /// <returns></returns>
        string DeleteContentUpdate(long contentLogId, ref ErrorResponseModel errorResponseModel);

    }
}

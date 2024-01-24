using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for CampaignDetail related operations
    /// </summary>
    public interface ICampaignDetailService
    {
        /// <summary>
        /// Method is used to get CampaignDetail by id
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        CampaignDetailModel GetCampaignDetailById(long marketingcampaignId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get CampaignDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<CampaignDetailModel> GetCampaignDetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all CampaignDetail
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<CampaignDetailModel> GetAllCampaignDetail(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to add/edit CampaignDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddCampaignDetail(CampaignDetailModel model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete CampaignDetail
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        string DeleteCampaignDetail(long marketingcampaignId, ref ErrorResponseModel errorResponseModel);

       
    }
}

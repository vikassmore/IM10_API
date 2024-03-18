using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for CampaignSocialMediaDetail related operations
    /// </summary>
    public interface ICampaignSocialMediaDetailService
    {
        /// <summary>
        /// Method is used to get CampaignSocialMediaDetail by marketingcampaignId
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
       List< CampaignSocialMediaDetailModel> GetCampaignSocialMediaDetailById(long marketingcampaignId, ref ErrorResponseModel errorResponseModel);


        

        /// <summary>
        /// Method is used to add/edit CampaignSocialMediaDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddCampaignSocialMediaDetail(CampaignSocialMediaDetailModel model, ref ErrorResponseModel errorResponseModel);


    }
}

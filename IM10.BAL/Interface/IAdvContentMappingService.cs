using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for AdvContentMapping related operations
    /// </summary>
    public interface IAdvContentMappingService
    {
        /// <summary>
        /// Method is used to get AdvContentMapping by id
        /// </summary>
        /// <param name="AdvcontentmapId"></param>
        /// <returns></returns>
        AdvContentMappingModel1 GetAdvContentMappingById(long AdvcontentmapId, ref ErrorResponseModel errorResponseModel);
     
        /// <summary>
        /// Method is used to get all AdvContentMapping
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<AdvContentMappingModel1> GetAllAdvContentMapping(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add AdvContentMapping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        NotificationModel AddAdvContentMapping(AdvContentMappingModel model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete AdvContentMapping
        /// </summary>
        /// <param name="advcontentmapId"></param>
        /// <returns></returns>
        string DeleteAdvContentMapping(long advcontentmapId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get AdvContentMapping by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<  AdvContentMappingModel1> GetAdvContentMappingByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


       
    }
}

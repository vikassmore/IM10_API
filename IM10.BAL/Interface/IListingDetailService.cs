using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for ListingDetail related operations
    /// </summary>

    public interface IListingDetailService
      {
             /// <summary>
            /// Method is used to get ListingDetail by id
            /// </summary>
            /// <param name="listingId"></param>
            /// <returns></returns>
            ListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel);

        
            /// <summary>
            /// Method is used to get all ListingDetail
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            List<ListingDetailModel> GetAllListingDetail(ref ErrorResponseModel errorResponseModel);


            /// <summary>
            /// Method is used to add/edit ListingDetail
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            string AddListingDetail(ListingDetailModel1 model, ref ErrorResponseModel errorResponseModel);


            /// <summary>
            /// Method for delete ListingDetail
            /// </summary>
            /// <param name="listingId"></param>
            /// <returns></returns>
            string DeleteListingDetail(long listingId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
     List< ListingDetailModel> GetListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);


    }
}

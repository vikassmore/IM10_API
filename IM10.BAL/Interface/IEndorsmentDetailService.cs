using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for EndorsmentDetail related operations
    /// </summary>
    public interface IEndorsmentDetailService
    {
            /// <summary>
            /// Method is used to get EndorsmentDetail by endorsmentid
            /// </summary>
            /// <param name="endorsmentId"></param>
            /// <returns></returns>
            EndorsmentDetailModel GetEndorsmentDetailById(long endorsmentId, ref ErrorResponseModel errorResponseModel);

            /// <summary>
            /// Method is used to get EndorsmentDetail by playerid
            /// </summary>
            /// <param name="playerId"></param>
            /// <returns></returns>
            List<EndorsmentDetailModel> GetEndorsmentDetailPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);

            /// <summary>
            /// Method is used to get all EndorsmentDetail
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            List<EndorsmentDetailModel> GetAllEndorsmentDetail(ref ErrorResponseModel errorResponseModel);

            /// <summary>
            /// Method is used to add/edit EndorsmentDetail
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            string AddEndorsmentDetail(EndorsmentDetailModel model, ref ErrorResponseModel errorResponseModel);

            /// <summary>
            /// Method is used to delete EndorsmentDetail
            /// </summary>
            /// <param name="endorsmentId"></param>
            /// <returns></returns>
            string DeleteEndorsmentDetail(long endorsmentId, ref ErrorResponseModel errorResponseModel);

        }
    }

using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    public interface IEndorsmentTypeService
    {
        /// <summary>
        /// Method is used to get EndorsmentType by endorsmenttypeId
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        EndorsmentTypeModel GetEndorsmentTypeById(long endorsmenttypeId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get all EndorsmentType
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<EndorsmentTypeModel> GetAllEndorsmentType(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add/edit EndorsmentType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddEndorsmentType(EndorsmentTypeModel model, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to delete EndorsmentType
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        string DeleteEndorsmentType(long endorsmenttypeId, ref ErrorResponseModel errorResponseModel);

    }
}

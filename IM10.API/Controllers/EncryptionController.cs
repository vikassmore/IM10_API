using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace IM10.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : BaseAPIController
    {
        IEncryptionService _service;

        public EncryptionController(IEncryptionService service)
        {
            _service = service;
        }


        /// <summary>
        /// To GetEncryptedId 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetEncryptedId/{Id}")]
        [ProducesResponseType(typeof(ExploreData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetEncryptedId(string Id)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {

                var detailModel = _service.GetEncryptedId(Id);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To GetDecryotedId 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetDecryotedId/{Id}")]
        [ProducesResponseType(typeof(ExploreData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetDecryotedId(string Id)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {

                var detailModel = _service.GetDecryotedId(Id);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

    }
}

using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for advcontentdetails entity 
    /// </summary>


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AdvtContentDetailController : BaseAPIController
    {
        IAdvContentDetailService  advContentservice;
        private readonly IWebHostEnvironment iwebhostingEnvironment;

        /// <summary>
        /// Used to initialize controller and inject advcontentdetails service
        /// </summary>
        /// <param name="_advContentservice"></param>

        public AdvtContentDetailController(IAdvContentDetailService _advContentservice, IWebHostEnvironment _iwebhostingEnvironment)
        {
            advContentservice = _advContentservice;
            iwebhostingEnvironment = _iwebhostingEnvironment;
        }



        /// <summary>
        /// To get AdvContentDetail by AdvertiseContentId 
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <returns></returns>
        [HttpGet("GetAdvContentDetailById/{AdvertiseContentId}")]
        [ProducesResponseType(typeof(AdvContentDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAdvContentDetailById(long AdvertiseContentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (AdvertiseContentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = advContentservice.GetAdvContentDetailById(AdvertiseContentId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

       
        /// <summary>
        /// Create an AdvContentDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddAdvContentDetail")]
        [Authorize]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(AdvContentDetailsModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddAdvContentDetail(IFormCollection formdata)
        {
            AdvContentDetailsModel1 model= new AdvContentDetailsModel1();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.AdvertiseContentId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.AdvertiseContentId != 0 ? Convert.ToInt32(userId) : model.UpdatedBy;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }


            try
            {
                var errorMessage = new ErrorResponseModel();            
                {
                    model.AdvertiseContentId = Convert.ToInt32(formdata["advertisecontentId"]);
                    model.NationId = Convert.ToInt32(formdata["nationId"]);
                    model.StateId = Convert.ToInt32(formdata["stateId"]);
                    model.CityId = Convert.ToInt32(formdata["cityId"]);
                    model.ContentTypeId = Convert.ToInt32(formdata["contenttypeId"]);
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.Title = formdata["title"];
                    model.FinalPrice = formdata["finalPrice"];
                    model.StartDate =Convert.ToDateTime( formdata["startDate"]);
                    model.IsFree =Convert.ToBoolean(formdata["isFree"]);
                    model.EndDate =Convert.ToDateTime(formdata["endDate"]);
                    model.IsGlobal =Convert.ToBoolean( formdata["isGlobal"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.AdvertiseContentId != 0)
                    {
                        for (int i = 0; i < existingFilesNotForDelete.Count(); i++)
                        {

                            string filePath = existingFilesNotForDelete[i].ToString();
                            if (filePath == "")
                            {
                                break;
                            }
                            string[] pathArr = filePath.Split('\\');
                            string[] fileArr = pathArr.Last().Split('/');
                            string fileBaseName = fileArr.First().ToString();
                            string fileName = "/" + fileArr[3] + "/" + fileArr[4].ToString();
                            model.AdvertiseFileName = fileName;
                            model.AdvertiseFilePath = formdata["imageUrl"];
                        }
                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "AdvtContentFile");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(fileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                var dbPath = Path.Combine(folderName, uniqueFileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                model.AdvertiseFilePath = "/AdvtContentFile/" + uniqueFileName;
                                model.AdvertiseContentId = Convert.ToInt32(formdata["advertisecontentId"]);
                                model.AdvertiseFileName = uniqueFileName;
                                
                            }
                        }
                    }
                }
                string productModel = "";

                if (model.AdvertiseContentId == 0 || model.AdvertiseContentId != 0)
                    productModel = advContentservice.AddAdvContentDetail(model, ref errorMessage);
               
                if (!string.IsNullOrEmpty(productModel))
                {
                    return Ok(productModel);
                }

                return ReturnErrorResponse(errorMessage);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }

        }


        /// <summary>
        /// Delete an AdvContentDetail
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAdvContentDetail")]
        [ProducesResponseType(typeof(AdvContentDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteAdvContentDetail(long AdvertiseContentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = advContentservice.DeleteAdvContentDetail(AdvertiseContentId, ref errorResponseModel);

                if (contentModel != null)
                {
                    return Ok(contentModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Get all AdvContentDetail by playerid
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAdvContentByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(AdvContentDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAdvContentByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = advContentservice.GetAdvContentByPlayerId(playerId, ref errorResponseModel);

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
        /// method for Approve AdvContentDetail
        /// </summary>
        /// <param name="advertisecontentId"></param>
        /// <returns></returns>
        [HttpPut("ApproveAdvContentDetail/{advertisecontentId}")]
        [ProducesResponseType(typeof(AdvContentDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult ApproveAdvContentDetail(long advertisecontentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = advContentservice.ApproveAdvContentDetail(advertisecontentId, ref errorResponseModel);

                if (contentModel != null)
                {
                    return Ok(contentModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Get all approvedAdvContentDetail by playerid
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetApprovedAdvContentByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(AdvContentDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetApprovedAdvContentByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = advContentservice.GetApprovedAdvContentByPlayerId(playerId, ref errorResponseModel);

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
        /// DeniedContentupdateDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("DeniedAdvContentDetail")]
        [ProducesResponseType(typeof(AdvContentComment), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeniedAdvContentDetail(AdvContentComment model)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var  advcontentModel = advContentservice.DeniedAdvContentDetail(model, ref errorResponseModel);

                if (advcontentModel != null)
                {
                    return Ok(advcontentModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

    }
}

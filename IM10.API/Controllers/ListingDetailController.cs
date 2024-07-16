using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for listingdetails entity 
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListingDetailController : BaseAPIController
    {
        IListingDetailService listingDetailService;
        private readonly IWebHostEnvironment iwebhostingEnvironment;

        /// <summary>
        /// Used to initialize controller and inject listingdetails service
        /// </summary>
        /// <param name="_listingDetailService"></param>
        public ListingDetailController(IListingDetailService _listingDetailService, IWebHostEnvironment _iwebhostingEnvironment)
        {
            listingDetailService = _listingDetailService;
            iwebhostingEnvironment = _iwebhostingEnvironment;
        }

        /// <summary>
        /// To get ListingDetail by listingId 
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns></returns>
        [HttpGet("GetListingDetailById/{listingId}")]
        [ProducesResponseType(typeof(ListingDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetListingDetailById(long listingId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (listingId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = listingDetailService.GetListingDetailById(listingId, ref errorResponseModel);

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
        /// Create an listingDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddListingDetail")]
        [Authorize(Roles = "Sales Person Admin,Endorsement Manager Admin")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ListingDetailModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddListingDetail(IFormCollection formdata)
        {

            ListingDetailModel1 model = new ListingDetailModel1();
             if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.ListingId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.ListingId != 0 ? Convert.ToInt32(userId) : model.CreatedBy;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }


            try
            {
                var errorMessage = new ErrorResponseModel();
                {

                    model.ListingId = Convert.ToInt32(formdata["listingId"]);
                    model.CompanyName = formdata["companyName"];
                    model.Description = formdata["description"];
                    model.ContactPersonName = formdata["contactpersonName"];
                    model.ContactPersonEmailId = formdata["contactpersonEmail"];
                    model.ContactPersonMobile = formdata["contactpersonMobile"];
                    model.CompanyEmailId = formdata["companyemailId"];
                    model.CompanyMobile = formdata["companyMobile"];
                    model.CompanyPhone = formdata["companyPhone"];
                    model.CompanyWebSite = formdata["companyWebsite"];
                    model.NationId = Convert.ToInt32(formdata["nationId"]);
                    model.StateId = Convert.ToInt32(formdata["stateId"]);
                    model.CityId = Convert.ToInt32(formdata["cityId"]);
                    model.IsGlobal = Convert.ToBoolean(formdata["isGlobal"]);
                    model.StartDate = Convert.ToDateTime(formdata["startDate"]);
                    model.EndDate = Convert.ToDateTime(formdata["endDate"]);
                    model.FinalPrice = formdata["finalPrice"];
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.Position = Convert.ToInt32(formdata["position"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.ListingId != 0)
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
                            model.CompanyLogoFileName = fileName;
                            model.CompanyLogoFilePath = formdata["imageUrl"];

                        }

                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "CompanyLogoFile");
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
                                model.CompanyLogoFilePath = "/CompanyLogoFile/" + uniqueFileName;
                                model.ListingId = Convert.ToInt32(formdata["listingId"]);
                                model.CompanyLogoFileName = uniqueFileName;

                            }
                        }
                    }
                }
                string productModel = "";

                if (model.ListingId == 0 || model.ListingId != 0)
                    productModel = listingDetailService.AddListingDetail(model, ref errorMessage);

                if (!string.IsNullOrEmpty(productModel))
                {
                    if (productModel != "Player Id does not exist")
                    {
                        return Ok(productModel);
                    }
                    else if (productModel == "Player Id does not exist")
                    {
                        return NotFound(productModel);
                    }
                }

                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }


        /// <summary>
        /// Delete an listingDetail
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteListingDetail")]
        [Authorize(Roles = "Sales Person Admin")]
        [ProducesResponseType(typeof(ListingDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteListingDetail(long listingId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var listingModel = listingDetailService.DeleteListingDetail(listingId, ref errorResponseModel);

                if (listingModel != null)
                {
                    return Ok(listingModel);
                }
                else if (errorResponseModel.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(errorResponseModel.Message);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get listingDetails by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetListingDetailByplayerId/{playerId}")]
        [ProducesResponseType(typeof(ListingDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetListingDetailByplayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var listdetailModel = listingDetailService.GetListingDetailByplayerId(playerId, ref errorResponseModel);

                if (listdetailModel != null)
                {
                    return Ok(listdetailModel);
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
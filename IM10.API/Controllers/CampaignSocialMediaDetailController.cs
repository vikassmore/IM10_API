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
    /// APIs for CampaignSocialMediaDetail entity 
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignSocialMediaDetailController : BaseAPIController
    {
        ICampaignSocialMediaDetailService service;
        private readonly IWebHostEnvironment iwebhostingEnvironment;


        /// <summary>
        /// Used to initialize controller and inject CampaignSocialMediaDetail service
        /// </summary>
        /// <param name="_detailservice"></param>
        public CampaignSocialMediaDetailController(ICampaignSocialMediaDetailService _detailservice, IWebHostEnvironment _iwebhostingEnvironment)
        {
            service = _detailservice;
            iwebhostingEnvironment = _iwebhostingEnvironment;

        }

        /// <summary>
        /// To get CampaignSocialMediaDetail by marketingcampaignId 
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        [HttpGet("GetCampaignSocialMediaDetailById/{marketingcampaignId}")]
        [ProducesResponseType(typeof(CampaignSocialMediaDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCampaignSocialMediaDetailById(long marketingcampaignId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (marketingcampaignId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = service.GetCampaignSocialMediaDetailById(marketingcampaignId, ref errorResponseModel);

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
        /// Create/Update an CampaignSocialMediaDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddCampaignSocialMediaDetail")]
        [Authorize]
        [ProducesResponseType(typeof(CampaignSocialMediaDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddCampaignSocialMediaDetail(IFormCollection formdata)
        {
            CampaignSocialMediaDetailModel model = new CampaignSocialMediaDetailModel();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.MarketingCampaignId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.MarketingCampaignId != 0 ? Convert.ToInt32(userId) : model.UpdatedBy;
            }


            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                {

                    model.CampaignId = Convert.ToInt32(formdata["campaignId"]);
                    model.SocialMediaViews = formdata["socialmediaview"];                  
                    model.MarketingCampaignId = Convert.ToInt32(formdata["marketingcampaignId"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.CampaignId != 0)
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
                            model.ScreenShotFileName = fileName;
                            model.ScreenShotFilePath = formdata["imageUrl"];

                        }

                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "ScreenShotFile");
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
                                model.ScreenShotFilePath = "/ScreenShotFile/" + uniqueFileName;
                                model.CampaignId = Convert.ToInt32(formdata["campaignId"]);
                                model.ScreenShotFileName = uniqueFileName;

                            }
                        }
                    }
                }
                string productModel = "";

                if (model.CampaignId == 0)
                    productModel = service.AddCampaignSocialMediaDetail(model, ref errorMessage);
               
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


    }
}

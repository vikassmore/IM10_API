using IM10.API.Hubs;
using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IM10.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAppController : BaseAPIController
    {
        IMobileServices services;
        private readonly IHubContext<NotificationsHubService> _hubContext;

        /// <summary>
        /// Used to initialize controller and inject mobileapp service
        /// </summary>
        /// <param name="_services"></param>
        public MobileAppController(IMobileServices _services, IHubContext<NotificationsHubService> hubContext)
        {
            services = _services;
            _hubContext = hubContext;
        }

        /// <summary>
        /// To get ContentflagDetail by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetTop5TrendingVideoContent/{playerId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetTop5TrendingVideoContent(long playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetTop5TrendingVideoContent(playerId, userId, ref errorResponseModel);

                if (detailModel.Count != 0)
                {
                    foreach (var item in detailModel)
                    {
                       await _hubContext.Clients.Group(playerId.ToString()).SendAsync("TrendingNotification", playerId, item.ContentId, item.Title, item.Description);
                    }
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
        /// To get MobileVideoView by contentId 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileVideoView/{contentId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileVideoView(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (contentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetMobileVideoView(contentId, ref errorResponseModel);

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
        /// To get all Category top five video by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryTopFiveVideoContent/{playerId}/{userId}")]
        [ProducesResponseType(typeof(MobileVideoCategoryData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategoryTopFiveVideoContent(long playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetAllCategoryTopFiveVideoContent(playerId, userId, ref errorResponseModel);

                if (detailModel.Count != 0)
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
        /// To get all video By Category
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("GetVideoContentByCategory/{playerId}/{categoryId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetVideoContentByCategory(long playerId, long categoryId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetVideoContentByCategory(playerId, categoryId,userId, ref errorResponseModel);

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
        /// To get all Category top five Article by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryTopFiveArticleContent/{playerId}/{userId}")]
        [ProducesResponseType(typeof(MobileArticleCategoryData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategoryTopFiveArticleContent(long playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetAllCategoryTopFiveArticleContent(playerId,userId, ref errorResponseModel);

                if (detailModel.Count != 0)
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
        /// To get all article By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetArticleContentByCategory/{playerId}/{categoryId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetArticleContentByCategory(long playerId, long categoryId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetArticleContentByCategory(playerId, categoryId,userId, ref errorResponseModel);

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
        /// To get MobileArticleView by contentId 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileArticleView/{contentId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileArticleView(long contentId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (contentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetMobileArticleView(contentId,userId, ref errorResponseModel);

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
        /// To get all ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetListingDetailByplayerId/{playerId}")]
        [ProducesResponseType(typeof(ListingLogoDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
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
                var detailModel = services.GetListingDetailByplayerId(playerId, ref errorResponseModel);

                if (detailModel.Count != 0)
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
        /// To get ListingDetail by Listingid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetListingDetailById/{listingId}")]
        [ProducesResponseType(typeof(ListingDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
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
                var detailModel = services.GetListingDetailById(listingId, ref errorResponseModel);

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
        /// To get To20ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetTop20ListingDetailByplayerId/{playerId}")]
        [ProducesResponseType(typeof(ListingLogoDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetTop20ListingDetailByplayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetTop20ListingDetailByplayerId(playerId, ref errorResponseModel);

                if (detailModel.Count != 0)
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
        /// Get the search data by playerId and title
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="searchData"></param>
        /// <returns></returns>
        [HttpGet("GetMobileSearchDetailByplayerId/{playerId}/{searchData}/{userId}")]
        [ProducesResponseType(typeof(MobileSearchDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSearchDetailByplayerId(long playerId, string searchData, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetMobileSearchDetailByplayerId(playerId, searchData,userId, ref errorResponseModel);

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
        /// Method is used to add/edit comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddMobileContentComment")]
        [ProducesResponseType(typeof(ContentCommentModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddMobileContentComment(ContentCommentModel1 model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.UpdatedBy = Convert.ToInt32(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var contentModel = services.AddMobileContentComment(model, ref errorMessage);
                if (contentModel != "")
                {
                    var jsonContent = JsonConvert.SerializeObject(contentModel);
                    return Ok(jsonContent);
                }
                return ReturnErrorResponse(errorMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get contentcomment by contentId
        /// </summary>
        /// <param name="">contentId</param>
        /// <returns></returns>
        [HttpGet("GetMobileCommentByContentId/{contentId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileCommentByContentId(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var commentModel = services.GetMobileCommentByContentId(contentId, ref errorResponseModel);
                if (commentModel.Count != 0)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Get contentcomment by playerId
        /// </summary>
        /// <param name="">playerId</param>
        /// <returns></returns>
        [HttpGet("GetMobileCommentByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileCommentByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var commentModel = services.GetMobileCommentByPlayerId(playerId, ref errorResponseModel);

                if (commentModel.Count != 0)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Method is used to add view flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddMobileViewCount")]
        [ProducesResponseType(typeof(ContentModelView), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddMobileViewCount(ContentModelView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var contentModel = services.AddMobileViewCount(model, ref errorMessage);
                if (contentModel != "")
                {
                    var jsonContent = JsonConvert.SerializeObject(contentModel);
                    return Ok(jsonContent);
                }
                return ReturnErrorResponse(errorMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }
        /// <summary>
        /// Method is used to add like flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddMobileLikeFavouriteCount")]
        [ProducesResponseType(typeof(ContentModelFlag), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddMobileLikeFavouriteCount(ContentModelFlag model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var contentModel = services.AddMobileLikeFavouriteCount(model, ref errorMessage);
                if (contentModel != "")
                {
                    var jsonContent = JsonConvert.SerializeObject(contentModel);
                    return Ok(jsonContent);
                }
                return ReturnErrorResponse(errorMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }
        /// <summary>
        /// To get splash screen by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileSplashScreenByplayerId/{playerId}")]
        [ProducesResponseType(typeof(PlayerMobileDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSplashScreenByplayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetMobileSplashScreenByplayerId(playerId, ref errorResponseModel);

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
        /// To get logo image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileLogoImageByplayerId/{playerId}")]
        [ProducesResponseType(typeof(PlayerMobileDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileLogoImageByplayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetMobileLogoImageByplayerId(playerId, ref errorResponseModel);

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
        /// To get get slide image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileSlideImageByplayerId/{playerId}")]
        [ProducesResponseType(typeof(PlayerMobileDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSlideImageByplayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetMobileSlideImageByplayerId(playerId, ref errorResponseModel);

                if (detailModel.Count != 0)
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
        /// To get liked video and article by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileLikeVideoArticle/{playerId}/{userId}")]
        [ProducesResponseType(typeof(PlayerMobileLikeFavouriteData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileLikeVideoArticle(long playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetMobileLikeVideoArticle(playerId, userId, ref errorResponseModel);

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
        /// To get favourite video and article by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileFavouriteVideoArticle/{playerId}/{userId}")]
        [ProducesResponseType(typeof(PlayerMobileLikeFavouriteData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileFavouriteVideoArticle(long playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = services.GetMobileFavouriteVideoArticle(playerId, userId, ref errorResponseModel);

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

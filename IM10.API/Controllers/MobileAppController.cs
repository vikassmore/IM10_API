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
        /// To get ContentflagDetail by playerId, userid and countNumber
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="countNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTop5TrendingVideoContent/{playerId}/{userId}/{countNumber}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetTop5TrendingVideoContent(string playerId, long userId, int countNumber)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
                var detailModel = services.GetTop5TrendingVideoContent(playerId, userId, countNumber, ref errorResponseModel);

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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileVideoView/{contentId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileVideoView(long contentId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel() ;
            try
            {
                if (contentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = services.GetMobileVideoView(contentId,userId ,ref errorResponseModel);

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
        /// To get all Category top five video by playerId and userid 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryTopFiveVideoContent/{playerId}/{userId}")]
        [ProducesResponseType(typeof(MobileVideoCategoryData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategoryTopFiveVideoContent(string playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
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
        /// To get all video By Categoryid, playerid ,userid and countNumber
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="countNumber"></param>
        /// <returns></returns>
        [HttpGet("GetVideoContentByCategory/{playerId}/{categoryId}/{userId}/{countNumber}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetVideoContentByCategory(string playerId, long categoryId, long userId,int countNumber)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var contentdetailModel = services.GetVideoContentByCategory(playerId, categoryId,userId, countNumber,ref errorResponseModel);

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
        /// To get all Category top five Article by playerId and countNumber
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="countNumber"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryTopFiveArticleContent/{playerId}/{userId}/{countNumber}")]
        [ProducesResponseType(typeof(MobileArticleCategoryData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategoryTopFiveArticleContent(string playerId, long userId, int countNumber)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
                var detailModel = services.GetAllCategoryTopFiveArticleContent(playerId,userId, countNumber, ref errorResponseModel);

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
        /// To get all article content By Categoryid,playerid and userid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetArticleContentByCategory/{playerId}/{categoryId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetArticleContentByCategory(string playerId, long categoryId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
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
        /// To get MobileArticleView by contentId and userid
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileArticleView/{contentId}/{userId}")]
        [ProducesResponseType(typeof(ContentFlagModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileArticleView(long contentId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetListingDetailByplayerId(string playerId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
               
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
        /// <param name="listingId"></param>
        /// <returns></returns>
        [HttpGet("GetListingDetailById/{listingId}")]
        [ProducesResponseType(typeof(MobileListingDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetListingDetailById(long listingId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
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
        /// To get To20ListingDetail by playerId and countNumber
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="countNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTop20ListingDetailByplayerId/{playerId}/{countNumber}")]
        [ProducesResponseType(typeof(ListingLogoDetailsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetTop20ListingDetailByplayerId(string playerId,int countNumber)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
                var detailModel = services.GetTop20ListingDetailByplayerId(playerId,countNumber, ref errorResponseModel);

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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileSearchDetailByplayerId/{playerId}/{searchData}/{userId}")]
        [ProducesResponseType(typeof(MobileSearchDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSearchDetailByplayerId(string playerId, string searchData, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
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
        [ProducesResponseType(typeof(string), 401)]
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
        /// Get contentcomment by contentId and userid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileCommentByContentId/{contentId}/{UserId}")]
        [ProducesResponseType(typeof(CommentListData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileCommentByContentId(long contentId,long UserId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var commentModel = services.GetMobileCommentByContentId(contentId, UserId,ref errorResponseModel);
                if (commentModel != null)
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
        /// Get MobileCommentCount by contentId and userid
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileCommentCount/{contentId}/{userId}")]
        [ProducesResponseType(typeof(CommentCountData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileCommentCount(long contentId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var commentModel = services.GetMobileCommentCount (contentId, userId, ref errorResponseModel);
                if (commentModel != null)
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
        /// <param name="playerId</param>
        /// <returns></returns>
        [HttpGet("GetMobileCommentByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileCommentByPlayerId(string playerId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
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
        [ProducesResponseType(typeof(string), 401)]
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
        [ProducesResponseType(typeof(string), 401)]
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSplashScreenByplayerId(string playerId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileLogoImageByplayerId(string playerId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileSlideImageByplayerId(string playerId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
               
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileLikeVideoArticle(string playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                
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
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileFavouriteVideoArticle(string playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
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


        /// <summary>
        /// To Get All CategoryList
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryList/{playerId}/{userId}")]
        [ProducesResponseType(typeof(ExploreData), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAllCategoryList(string playerId, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var detailModel = services.GetAllCategoryList(playerId, userId, ref errorResponseModel);

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
        /// To GetAllPlayerList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllPlayerList")]
        [ProducesResponseType(typeof(PlayerListModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAllPlayerList()
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var detailModel = services.GetAllPlayerList(ref errorResponseModel);

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
        /// To GetCommonAppPlayerList
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCommonAppPlayerList")]
        [ProducesResponseType(typeof(ExploreCategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCommonAppPlayerList()
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var detailModel = services.GetCommonAppPlayerList(ref errorResponseModel);

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
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetCommonAppSearchDetailByplayerId/{playerId}/{searchData}/{userId}")]
        [ProducesResponseType(typeof(MobileSearchDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCommonAppSearchDetailByplayerId(string playerId, string searchData, long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {

                var contentdetailModel = services.GetCommonAppSearchDetailByplayerId(playerId, searchData, userId, ref errorResponseModel);

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

    }
}

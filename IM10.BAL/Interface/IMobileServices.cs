using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for mobileapp related operations
    /// </summary>

    public interface IMobileServices
    {

        /// <summary>
        /// Method is used to get all treding contentflagDetail
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MobileContentData> GetTop5TrendingVideoContent(long playerId,long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all Category top five video
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MobileVideoCategoryData> GetAllCategoryTopFiveVideoContent(long playerId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all video By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileVideoCategoryData GetVideoContentByCategory(long playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get mobileVideoview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        ContentFlagModel GetMobileVideoView(long contentId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all Category top five Article
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MobileArticleCategoryData> GetAllCategoryTopFiveArticleContent(long playerId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all article By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileArticleCategoryData GetArticleContentByCategory(long playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get mobilearticleview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ContentFlagModel GetMobileArticleView(long contentId, long userId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ListingLogoDetailsModel> GetListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get ListingDetail by id
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns></returns>
        ListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get To20ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ListingLogoDetailsModel> GetTop20ListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get the serach data by playerId and title
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="searchData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileSearchDataModel GetMobileSearchDetailByplayerId(long playerId, string searchData, long userId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add/edit comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMobileContentComment(ContentCommentModel1 model, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contentcomment by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        List<ContentCommentModelData> GetMobileCommentByContentId(long contentId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contentcomment by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ContentCommentModel> GetMobileCommentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add view flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMobileViewCount(ContentModelView model, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add like and Favourite flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMobileLikeFavouriteCount(ContentModelFlag model, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get splash screen by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        PlayerMobileDataModel GetMobileSplashScreenByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get logo image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        PlayerMobileDataModel GetMobileLogoImageByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get slide image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<PlayerMobileDataModel> GetMobileSlideImageByplayerId(long playerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get the liked data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PlayerMobileLikeFavouriteData GetMobileLikeVideoArticle(long playerId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get the favourite data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PlayerMobileLikeFavouriteData GetMobileFavouriteVideoArticle(long playerId, long userId, ref ErrorResponseModel errorResponseModel);
    }
}

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
        List<MobileContentData> GetTop5TrendingVideoContent(string playerId,long userId,int countNumber, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all Category top five video
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MobileVideoCategoryData> GetAllCategoryTopFiveVideoContent(string playerId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all video By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileVideoCategoryData GetVideoContentByCategory(string playerId, long categoryId, long userId, int countNumber, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get mobileVideoview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        ContentFlagModel GetMobileVideoView(long contentId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all Category top five Article
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<MobileArticleCategoryData> GetAllCategoryTopFiveArticleContent(string playerId, long userId, int countNumber, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all article By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileArticleCategoryData GetArticleContentByCategory(string playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel);
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
        List<ListingLogoDetailsModel> GetListingDetailByplayerId(string playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get ListingDetail by id
        /// </summary>
        /// <param name="listingId"></param>
        /// <returns></returns>
        MobileListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get To20ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ListingLogoDetailsModel> GetTop20ListingDetailByplayerId(string playerId, int countNumber, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get the serach data by playerId and title
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="searchData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        MobileSearchDataModel GetMobileSearchDetailByplayerId(string playerId, string searchData, long userId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add/edit comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMobileContentComment(ContentCommentModel1 model, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contentcomment by contentId,userid
        /// </summary>
        /// <param name="contentId"></param>\
        /// <param name="errorResponseModel"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        CommentListData GetMobileCommentByContentId(long contentId,long UserId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to GetMobileCommentCount by contentId,userid
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        CommentCountData GetMobileCommentCount(long contentId, long userId, ref ErrorResponseModel errorResponseModel);



        /// <summary>
        /// Method is used to get contentcomment by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<ContentCommentModel> GetMobileCommentByPlayerId(string playerId, ref ErrorResponseModel errorResponseModel);

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
        PlayerMobileDataModel GetMobileSplashScreenByplayerId(string playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get logo image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        PlayerMobileDataModel GetMobileLogoImageByplayerId(string playerId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get slide image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        List<PlayerMobileDataModel> GetMobileSlideImageByplayerId(string playerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get the liked data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PlayerMobileLikeFavouriteData GetMobileLikeVideoArticle(string playerId, long userId, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get the favourite data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PlayerMobileLikeFavouriteData GetMobileFavouriteVideoArticle(string playerId, long userId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get the GetAllCategoryList
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<ExploreData> GetAllCategoryList(string playerId, long userId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get the GetAllPlayerList
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        List<PlayerListModel> GetAllPlayerList(ref ErrorResponseModel errorResponseModel);
    }
}

using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for contentcomments related operations
    /// </summary>
    public interface IContentCommentService
    {

        /// <summary>
        /// Method is used to get contentcomment by commentId
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        ContentCommentModel GetContentCommentById(long commentId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get contentcomment by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
       List< ContentCommentModel> GetContentCommentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add commentreply
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<CommentNotificationModel> AddContentCommentReply(ContentCommentModel model);


        /// <summary>
        /// Method is used to delete commentreply
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        string DeleteCommentReply(long commentId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contentcomment reply by commentId
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        List<ContentCommentModel> GetContentCommentReplyByCommentId(long commentId, ref ErrorResponseModel errorResponseModel);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class NotificationModel
    {
        public long ContentId { get; set; }
        public long PlayerId { get; set; }
        public int ContentTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string Thumbnail { get; set; }
    }

    public class CommentNotificationModel
    {
        public long CommentId { get; set; }
        public long ContentId { get; set; }
        public int ContentTypeId { get; set; }
        public string? title { get; set; }
        public string Message { get; set; }

    }
}

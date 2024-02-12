using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class FCMNotificationModel
    {
        public long FcmnotificationId { get; set; }
        public string? DeviceToken { get; set; }
        public long PlayerId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }


    public class FcmNotificationSetting
    {
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
    }

    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

}


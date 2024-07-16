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
        public long UserId { get; set; }
        public int? CreatedBy { get; set; }
    }



    public class FcmNotificationSetting
    {
        public string SenderId { get; set; }
        public string ServerKey { get; set; }

        public string private_key { get; set; }
        public string client_email { get; set; }
        public string token_uri { get; set; }
    }

    public class ResponseModel
    {
        public int IsSuccess { get; set; }
        public string Message { get; set; }
    }

}


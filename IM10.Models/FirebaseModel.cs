using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class FirebaseModel
    {
       
        public class FirebaseNotificationModel
        {
            [JsonProperty("deviceId")]
            public string DeviceId { get; set; }
            [JsonProperty("isAndroiodDevice")]
            public bool IsAndroiodDevice { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("body")]
            public string Body { get; set; }
        }
        public class GoogleNotification
        {
            public class DataPayload
            {
                [JsonProperty("playerId")]
                public long PlayerId { get; set; }

                [JsonProperty("contentId")]
                public long contentId { get; set; }

                [JsonProperty("title")]
                public string Title { get; set; }

                [JsonProperty("description")]
                public string Description { get; set; }

                [JsonProperty("commentId")]
                public long commentId { get; set; }

                [JsonProperty("message")]
                public string Message { get; set; }

            }
            [JsonProperty("data")]
            public DataPayload Data { get; set; }
            [JsonProperty("notification")]
            public DataPayload Notification { get; set; }

        }
    }
}


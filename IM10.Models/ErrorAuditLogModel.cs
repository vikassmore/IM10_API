using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ErrorAuditLogModel
    {
        public long LogId { get; set; }
        public string? LogType { get; set; }
        public string? StackTrace { get; set; }
        public string? AdditionalInformation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LogSource { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string EmailId { get; set; } = null!;
        public string? LogMessage { get; set; }
    }
}

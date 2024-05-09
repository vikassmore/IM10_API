using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class UserAuditLogModel
    {
      //  public long AuditId { get; set; }

        public string Action { get; set; } = null!;

        public string Description { get; set; } = null!;

        public long UserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

    }
}

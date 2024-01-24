using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class PlayerDataModel
    {
        public int PlayerDataId { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public int FileCategoryTypeId { get; set; }

        public long PlayerId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

        public string? PlayerName { get; set; }
    }
}

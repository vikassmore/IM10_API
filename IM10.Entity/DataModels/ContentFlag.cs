using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class ContentFlag
{
    public long ContentFlagId { get; set; }

    public long PlayerId { get; set; }

    public long ContentId { get; set; }

    public bool? MostLiked { get; set; }

    public bool? Favourite { get; set; }

    public int? ContentSequence { get; set; }

    public int ContentTypeId { get; set; }

    public long UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ContentDetail Content { get; set; } = null!;

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual PlayerDetail Player { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}

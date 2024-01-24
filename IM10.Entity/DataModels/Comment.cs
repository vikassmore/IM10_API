using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class Comment
{
    public long CommentId { get; set; }

    public long UserId { get; set; }

    public long ContentId { get; set; }

    public int ContentTypeId { get; set; }

    public string? DeviceId { get; set; }

    public string? Location { get; set; }

    public bool? Liked { get; set; }

    public string? Comment1 { get; set; }

    public bool? Shared { get; set; }

    public bool? IsPublic { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public long? ParentCommentId { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ContentDetail Content { get; set; } = null!;

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual ICollection<Comment> InverseParentComment { get; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class Language
{
    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ContentDetail> ContentDetails { get; } = new List<ContentDetail>();
}

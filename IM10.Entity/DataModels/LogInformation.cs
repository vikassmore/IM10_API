using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class LogInformation
{
    public long LogId { get; set; }

    public string? LogType { get; set; }

    public string? StackTrace { get; set; }

    public string? AdditionalInformation { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? LogSource { get; set; }

    public int? UserId { get; set; }

    public string? LogMessage { get; set; }
}

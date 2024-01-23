using System;
using System.Collections.Generic;

namespace ImportData.Models;

public partial class Stdatum
{
    public string StudentId { get; set; } = null!;

    public string? Mathematics { get; set; }

    public string? Literature { get; set; }

    public string? Physics { get; set; }

    public string? Biology { get; set; }

    public string? English { get; set; }

    public string? Year { get; set; }

    public string? Chemistry { get; set; }

    public string? History { get; set; }

    public string? Geography { get; set; }

    public string? CivicEducation { get; set; }

    public string? Province { get; set; }
}

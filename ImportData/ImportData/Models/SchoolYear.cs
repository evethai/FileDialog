using System;
using System.Collections.Generic;

namespace ImportData.Models;

public partial class SchoolYear
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ExamYear { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}

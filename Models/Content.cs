using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Content
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string? ContentUrl { get; set; }

    public int? CourseId { get; set; }

    public virtual Course? Course { get; set; }
}

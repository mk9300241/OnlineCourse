using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public string? CourseDescription { get; set; }
    public string? Img { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual User? User { get; set; }
}

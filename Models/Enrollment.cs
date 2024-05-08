using System;
using System.Collections.Generic;

namespace WebApplication2.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public string? PaymentId { get; set; }

    public string? TransactionId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
}

using System;
using System.Collections.Generic;

namespace AttendEase.DB.Models;

public partial class SpecialDay
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public bool Recursive { get; set; }
}

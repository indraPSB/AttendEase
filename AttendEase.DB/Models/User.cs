using System;
using System.Collections.Generic;

namespace AttendEase.DB.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }
}

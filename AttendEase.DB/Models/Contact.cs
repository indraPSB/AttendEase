using System;
using System.Collections.Generic;

namespace AttendEase.DB.Models;

public partial class Contact
{
    public Guid Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string Email { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string? MessageUser { get; set; }

    public string? MessageSystem { get; set; }

    public string Status { get; set; } = null!;
}

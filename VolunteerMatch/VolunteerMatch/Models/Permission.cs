using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class Permission
{
    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

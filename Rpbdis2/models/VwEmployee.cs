using System;
using System.Collections.Generic;

namespace Rpbdis2.models;

public partial class VwEmployee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Education { get; set; } = null!;

    public string Position { get; set; } = null!;
}

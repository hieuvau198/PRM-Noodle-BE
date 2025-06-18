using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class user
{
    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password { get; set; } = null!;

    public string full_name { get; set; } = null!;

    public string? phone { get; set; }

    public string? address { get; set; }

    public string? role { get; set; }

    public bool? is_active { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}

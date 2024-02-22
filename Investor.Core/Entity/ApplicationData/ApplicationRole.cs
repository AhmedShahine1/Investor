using Microsoft.AspNetCore.Identity;

namespace Investor.Core.Entity.ApplicationData;

public class ApplicationRole : IdentityRole
{
    public string NameAr { get; set; }

    public string Description { get; set; }

    public int RoleNumber { get; set; }
}
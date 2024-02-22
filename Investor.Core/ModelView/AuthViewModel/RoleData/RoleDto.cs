using System.ComponentModel.DataAnnotations;

namespace Investor.Core.ModelView.AuthViewModel.RoleData;

public class RoleDto
{
    [Required]
    public string RoleName { get; set; }

    [Required]
    public string RoleNameAr { get; set; }

    public string Description { get; set; }

    public int GroupNumber { get; set; }
}
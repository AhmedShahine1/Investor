
using Investor.Core.Helpers;

namespace Investor.Core.ModelView.AuthViewModel;

public class AuthModel
{

    public bool IsAuthenticated { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool Status { get; set; } = true;
    public bool IsUser { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public int age { get; set; }

    public string Message { get; set; }
    public string ArMessage { get; set; }
    public int ErrorCode { get; set; }
    public string PhoneNumber { get; set; }
    public UserType UserType { get; set; }
    public string UserImgUrl { get; set; }
    public string Qualification { get; set; }
    public string Job { get; set; }
}
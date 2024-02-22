using Investor.Core.Entity.ApplicationData;
using Investor.Core.ModelView.AuthViewModel;
using Investor.Core.ModelView.AuthViewModel.ChangePasswordData;
using Investor.Core.ModelView.AuthViewModel.LoginData;
using Investor.Core.ModelView.AuthViewModel.RegisterData;
using Investor.Core.ModelView.AuthViewModel.RoleData;
using Investor.Core.ModelView.AuthViewModel.UpdateData;

namespace Investor.BusinessLayer.Interfaces;

public interface IAccountService
{
    Task<List<ApplicationUser>> GetAllUsers();
    Task<ApplicationUser> GetUserById(string userId);
    Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber);
    Task<ApplicationUser> GetUserByEmail(string email);
    Task<AuthModel> RegisterInvestor(RegisterInvestor model);
    Task<AuthModel> RegisterYouth(RegisterYouth model);
    Task<AuthModel> RegisterAdmin(RegisterAdmin model);
    Task<AuthModel> LoginAsync(LoginModel model);
    Task<bool> Logout(string userName);
    Task<AuthModel> ChangePasswordAsync(string userId, string password);
    Task<AuthModel> ChangeOldPasswordAsync(string userId, ChangeOldPassword changePassword);
    Task<AuthModel> UpdateUserProfile(string userId, UpdateUserMv updateUser);
    Task<AuthModel> GetUserInfo(string userId);
    Task<string> AddRoleAsync(AddRoleModel model);
    Task<List<string>> GetRoles();
 
    string ValidateJwtToken(string token);
    int GenerateRandomNo();
    //------------------------------------------------------
    Task Activate(string userId);
    Task Suspend(string userId);
    string RandomString(int length);
    Task<bool> DisActiveUserConnnection(string userId);
    Task<bool> ActiveUserConnnection(string userId);
}
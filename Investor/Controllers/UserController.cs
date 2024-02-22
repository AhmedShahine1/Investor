using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Investor.BusinessLayer.Interfaces;
using Investor.Core.DTO;
using Investor.Core.Entity.ApplicationData;
using Investor.Core.Helpers;
using Investor.Core.ModelView.AuthViewModel.LoginData;
using Investor.Core.ModelView.AuthViewModel.RegisterData;
using Investor.Core.ModelView.AuthViewModel.RoleData;
using Investor.RepositoryLayer.Interfaces;
using Investor.Core.ModelView.AuthViewModel.ChangePasswordData;
using Investor.Core.ModelView.AuthViewModel.UpdateData;
using Microsoft.EntityFrameworkCore;
using Investor.Core.Entity.ChatandUserConnection;
using Investor.Core.Entity.PostData;

namespace Investor.Controllers.API;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse = new();

    public UserController(IUnitOfWork unitOfWork, IAccountService accountService, RoleManager<ApplicationRole> roleManager)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
    }
    //----------------------------------------------------------------------------------------------
    [HttpPost("AddRole")]
    public async Task<ActionResult<BaseResponse>> AddRoles([FromForm] RoleDto roleDto, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var oldRole = await _roleManager.FindByNameAsync(roleDto.RoleName);
        if (oldRole != null)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الصلاحية موجودة مسبقا" : "This role is already exist";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }

        var role = await _roleManager.CreateAsync(new ApplicationRole()
        {
            Name = roleDto.RoleName,
            NameAr = roleDto.RoleNameAr,
            Description = roleDto.Description,
            RoleNumber = roleDto.GroupNumber,
        });
        if (role.Succeeded)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة الصلاحية بنجاح" : "Role added successfully";
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = role.Errors.ToString() };
            return Ok(_baseResponse);
        }
    }

    //-------------------------------------------------------------------------------------------- student Register 
    [HttpPost("InvestorRegister")]
    public async Task<ActionResult<BaseResponse>> InvestorRegister([FromForm] RegisterInvestor model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.RegisterInvestor(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new { result.FirstName, result.LastName, result.PhoneNumber, result.Email, result.UserImgUrl, result.Job, result.Qualification };
        }
        return Ok(_baseResponse);
    }

    [HttpPost("YouthRegister")]
    public async Task<ActionResult<BaseResponse>> YouthRegister([FromForm] RegisterYouth model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.RegisterYouth(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new { result.FirstName, result.LastName, result.PhoneNumber, result.Email, result.UserImgUrl, result.Job, result.Qualification };
        }
        return Ok(_baseResponse);
    }

    [HttpPost("AdminRegister")]
    public async Task<ActionResult<BaseResponse>> AdminRegister([FromForm] RegisterAdmin model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.RegisterAdmin(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new { result.FirstName, result.LastName, result.PhoneNumber, result.Email, result.UserImgUrl, result.Job, result.Qualification };
        }
        return Ok(_baseResponse);
    }


    //----------------------------------------------------------------------------------------------------- update user profile
    [HttpPut("UpdateProfile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<BaseResponse>> UpdateProfile([FromForm] UpdateUserMv updateUser, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _accountService.UpdateUserProfile(userId, updateUser);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = updateUser;
            return BadRequest(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new
        {
            result.Email,
            result.FirstName,
            result.LastName,
            result.PhoneNumber,
            Role = result.Roles,
            result.UserType,
            result.Job,
            result.Qualification,
            result.UserImgUrl,
            result.Status,
        };
        return Ok(_baseResponse);
    }

    //-------------------------------------------------------------------------------------------- login Api 
    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse>> LoginAsync([FromForm] LoginModel model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.LoginAsync(model);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.Data = model;
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الدخول" : "Login Successfully";
        _baseResponse.Data = new
        {
            result.UserId,
            result.Email,
            result.Token,
            Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber,
            result.FirstName,
            result.LastName,
            result.UserType,
            result.Job,
            result.Qualification,
            result.Status,
        };
        await _accountService.ActiveUserConnnection(result.UserId);
        return Ok(_baseResponse);
    }

    //-------------------------------------------------------------------------------------------- logout Api 
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<BaseResponse>> LogoutAsync([FromHeader] string lang)
    {
        var userId = User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId)
            .FirstOrDefault();
        if (!string.IsNullOrEmpty(user.UserName))
        {
            var result = await _accountService.Logout(user.UserName);
            if (result)
            {
                _baseResponse.ErrorCode = 0;
                _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الخروج بنجاح " : "Signed out successfully";
                await _accountService.DisActiveUserConnnection(userId);
                return Ok(_baseResponse);
            }
        }
        _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
        _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الحساب غير موجود " : "The User Not Exist";
        return Ok(_baseResponse);
    }

    //------------------------------------------------------------------------------------------------ Change Password Api
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("changeoldPassword")]
    public async Task<ActionResult<BaseResponse>> ChangeOldPasswordAsync([FromForm] ChangeOldPassword changePassword, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId

        var result = await _accountService.ChangeOldPasswordAsync(userId, changePassword);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new
        {
            result.Email,
            result.FirstName,
            result.LastName,
            result.PhoneNumber,
            Role = result.Roles,
            result.UserType,
            result.Job,
            result.Qualification,
            result.UserImgUrl,
            result.Status,
            result.Token,
        };
        return Ok(_baseResponse);
    }

    //------------------------------------------------------------------------------------------------ forgot Password Api
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("changeForgotPassword")]
    public async Task<ActionResult<BaseResponse>> ChangePasswordAsync([FromForm] ChangePasswordMv password, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        //var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userName
        var result = await _accountService.ChangePasswordAsync(userId, password.Password);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new
        {
            result.Email,
            result.FirstName,
            result.LastName,
            result.PhoneNumber,
            Role = result.Roles,
            result.UserType,
            result.Job,
            result.Qualification,
            result.UserImgUrl,
            result.Status,
        };//Token = result.Token,
        return Ok(_baseResponse);
    }

    [HttpDelete("DeleteAccount")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<BaseResponse>> DeleteAccount([FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (userId == null)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        await _accountService.Suspend(userId);
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف الحساب" : "Success Delete Account";
        _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
        _baseResponse.Data = userId;

        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------- get profile
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetUserInfo")]
    public async Task<ActionResult<BaseResponse>> GetUserInfo([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (string.IsNullOrEmpty(userId))
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود" : "User not exist";
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        var result = await _accountService.GetUserInfo(userId);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = result;
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new
        {
            result.Email,
            result.FirstName,
            result.LastName,
            result.PhoneNumber,
            Role = result.Roles,
            result.UserType,
            result.Job,
            result.Qualification,
            result.UserImgUrl,
            result.Status,
        };
        return Ok(_baseResponse);
    }

    [HttpGet("GetUserByID")]
    public async Task<ActionResult<BaseResponse>> GetUser(string userId, [FromHeader] string lang)
    {
        var UserDetail = _unitOfWork.Users.FindByQuery(s => s.Id == userId);
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = "null";
        _baseResponse.Data = new
        {
            UserDetails = UserDetail,
        };
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------- get Connection
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetUserConnection")]
    public async Task<ActionResult<BaseResponse>> GetUserConnnection([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (string.IsNullOrEmpty(userId))
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود" : "User not exist";
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        var UserConnection = await _unitOfWork.UserConnections.FindByQuery(s => s.UserId == userId && s.Connection == true).Select(s => new { s.UserConnectionId, s.UserId, s.ConnectionTime }).OrderByDescending(u => u.ConnectionTime).ToListAsync();
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data=UserConnection;
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------- get Connection
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("CheckUserConnection")]
    public async Task<ActionResult<BaseResponse>> CheckUserConnnection([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (string.IsNullOrEmpty(userId))
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود" : "User not exist";
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        var UserConnection = await _unitOfWork.UserConnections.FindByQuery(s => s.UserId == userId).Select(s => new { s.UserConnectionId, s.UserId, s.ConnectionTime , s.Connection}).OrderByDescending(u => u.ConnectionTime).ToListAsync();
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = UserConnection[0];
        return Ok(_baseResponse);
    }

}
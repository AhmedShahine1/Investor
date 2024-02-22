using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Investor.Core.DTO;
using Investor.Core.Entity.ApplicationData;
using Investor.Core.Helpers;
using Investor.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Investor.Core.DTO.EntityDto;
using Investor.Core.Entity.EvaluationData;

namespace Investor.Controllers.API;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EvaluationsController : BaseController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public EvaluationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(accessToken))
            return;

        var userId = User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId && s.Status == false)
            .FirstOrDefault();
        _user = user;
        
    }

        
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {
 
    }
    //---------------------------------------------------------------------------------------------------

    [HttpPost("AddUserdEvaluations")]
    public async Task<IActionResult> AddUserEvaluations([FromHeader]string lang , [FromForm] UserEvaluationDto model)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }
        


        var TargetUser = await _unitOfWork.Users.FindByQuery(s => s.Id == model.TargetUserId && s.Status == true).FirstOrDefaultAsync();
        if (TargetUser == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")? "هذه المستخدم غير موجود أو  تم حذف الحساب " : "The User Not Exist Or Deleted";
            return Ok(_baseResponse);

        }
        var userEvaluation = await _unitOfWork.EvaluationUser.FindByQuery(s => s.TargetUserId == TargetUser.Id && s.UserId == _user.Id).FirstOrDefaultAsync();

        if (userEvaluation != null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceAlreadyEvaluated;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لقد قمت بتقييم هذا المستخدم من قبل " : "The user Already Evaluated";
            return Ok(_baseResponse);
        }
        userEvaluation = new EvaluationUser
        {
            TargetUserId = TargetUser.Id,
            UserId = _user.Id,
            NumberOfStars = model.NumberOfStars,
        };
        await _unitOfWork.EvaluationUser.AddAsync(userEvaluation);
        await _unitOfWork.SaveChangesAsync();
        
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة التقييم بنجاح"
            : "The evaluation Has Been Added Successfully";

        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    [HttpGet("GetUserdEvaluations")]
    public async Task<IActionResult> GetUserEvaluations([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var userEvaluation = await _unitOfWork.EvaluationUser.FindByQuery(s => s.TargetUserId == _user.Id ).ToListAsync();
        double averageRating = userEvaluation.Average(r => r.NumberOfStars);
        double allRating = userEvaluation.Sum(r => r.NumberOfStars);
        var Rating = new
        {
            UserId = _user.Id,
            AverageRating = averageRating,
            AllRating = allRating
        };
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = Rating;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    [HttpGet("GetTargetUserdEvaluations")]
    public async Task<IActionResult> GetTargetUserEvaluations([FromHeader] string lang, [FromForm] string id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var userEvaluation = await _unitOfWork.EvaluationUser.FindByQuery(s => s.TargetUserId == id).ToListAsync();
        double averageRating = userEvaluation.Average(r => r.NumberOfStars);
        double allRating = userEvaluation.Sum(r => r.NumberOfStars);
        var Rating = new
        {
            UserId = _user.Id,
            AverageRating = averageRating,
            AllRating = allRating
        };
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = Rating;
        return Ok(_baseResponse);
    }
}
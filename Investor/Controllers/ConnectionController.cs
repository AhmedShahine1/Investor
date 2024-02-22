using Investor.BusinessLayer.Interfaces;
using Investor.Core.DTO;
using Investor.Core.DTO.EntityDTO;
using Investor.Core.Entity.ApplicationData;
using Investor.Core.Entity.ConnectionData;
using Investor.Core.Helpers;
using Investor.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace Investor.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConnectionController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ApplicationUser _user;

        public ConnectionController(IUnitOfWork unitOfWork, IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _baseResponse = new BaseResponse();
            _httpContextAccessor = httpContextAccessor;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(accessToken))
                return;

            var userId = User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
            var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId)
                .FirstOrDefault();
            _user = user;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        //--------------------------------------------------------------------------------------------------------
        // Add Request Connection
        [HttpPost("AddConnection")]
        public async Task<ActionResult<BaseResponse>> AddConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var Connection = await _unitOfWork.Connections.FindByQuery(s => (s.User1Id == _user.Id && s.User2Id == connectionDTO.TargetUserId) || (s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId)).FirstOrDefaultAsync();
            if (Connection != null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " تم ارسال طلب الصداقه من قبل "
                    : "The users are send connection ";
                return Ok(_baseResponse);
            }
            var Connect = new Connection
            {
                User1Id = _user.Id,
                User2Id = connectionDTO.TargetUserId,
            };
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم ارسال طلب الصداقه بنجاح"
                : "The user send request connection Successfully";

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Accept or Rejection Request Connection
        [HttpPut("AcceptConnection")]
        public async Task<ActionResult<BaseResponse>> AcceptConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var Connection = await _unitOfWork.Connections.FindByQuery(s => s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId).FirstOrDefaultAsync();
            if(Connection == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " لم يتم ارسال طلب الصداقه من قبل "
                    : "The user isn't send connection ";
                return Ok(_baseResponse);
            }

            if (connectionDTO.Agree == true)
            {
                Connection.IsAgree = true;

                _unitOfWork.Connections.Update(Connection);
                await _unitOfWork.SaveChangesAsync();

                _baseResponse.ErrorCode = (int)Errors.Success;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "تم قبول طلب الصداقه بنجاح"
                    : "The Connection Has Been Accepted Successfully";

                return Ok(_baseResponse);
            }
            else
            {
                _unitOfWork.Connections.Delete(Connection);
                await _unitOfWork.SaveChangesAsync();

                _baseResponse.ErrorCode = (int)Errors.Success;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "تم رفض طلب الصداقه بنجاح"
                    : "The Connection Has Been Rejection Successfully";
                return Ok(_baseResponse);
            }


        }

        //--------------------------------------------------------------------------------------------------------
        //  Cancel Connection
        [HttpDelete("CancelConnection")]
        public async Task<ActionResult<BaseResponse>> CancelConnection([FromHeader] string lang, [FromForm] ConnectionDTO connectionDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (!ModelState.IsValid)
            {
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var Connection = await _unitOfWork.Connections.FindByQuery(s => (s.User1Id == _user.Id && s.User2Id == connectionDTO.TargetUserId) || (s.User2Id == _user.Id && s.User1Id == connectionDTO.TargetUserId) && s.IsAgree == true).FirstOrDefaultAsync();
            if (Connection == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? " لم يتم ارسال طلب الصداقه من قبل "
                    : "The user isn't send connection ";
                return Ok(_baseResponse);
            }

            _unitOfWork.Connections.Delete(Connection);
            await _unitOfWork.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم الغاء طلب الصداقه بنجاح"
                : "The Connection Has Been Cancel Successfully";
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        //  Get all Accept Connection
        [HttpGet("AllAcceptConnection")]
        public async Task<ActionResult<BaseResponse>> GetAllConnection([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Connection = await _unitOfWork.Connections.FindByQuery(s => s.User1Id == _user.Id && s.User2Id == _user.Id  && s.IsAgree == true).FirstOrDefaultAsync();
            var data = new Connection
            {
                User1Id = Connection.User1Id,
                User2Id = Connection.User2Id,
            };
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = data;
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        //  Get all Wait response Connection
        [HttpGet("AllWaitConnection")]
        public async Task<ActionResult<BaseResponse>> GetAllWaitConnection([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Connection = await _unitOfWork.Connections.FindByQuery(s => s.User1Id == _user.Id && s.User2Id == _user.Id && s.IsAgree == false).FirstOrDefaultAsync();
            var data = new Connection
            {
                User1Id = Connection.User1Id,
                User2Id = Connection.User2Id,
            };
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = data;
            return Ok(_baseResponse);
        }

    }
}

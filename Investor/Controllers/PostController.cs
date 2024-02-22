using Investor.BusinessLayer.Interfaces;
using Investor.Core.DTO;
using Investor.Core.DTO.EntityDTO;
using Investor.Core.Entity.ApplicationData;
using Investor.Core.Entity.PostData;
using Investor.Core.Helpers;
using Investor.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Net.Mail;

namespace Investor.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : BaseController , IActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ApplicationUser _user;

        public PostController(IUnitOfWork unitOfWork, IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor)
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
            var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId && s.Status == false)
                .FirstOrDefault();
            _user = user;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        //--------------------------------------------------------------------------------------------------------
        // Create Category
        [HttpPost("AddCategory")]
        public async Task<ActionResult<BaseResponse>> AddCategory([FromHeader] string lang, [FromForm] Category category)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Admin)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  ادمن"
                    : "The User Not Admin ";
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

            var Category = _unitOfWork.Catagories.FindByQuery(s => s.CategoryName == category.CategoryName).FirstOrDefault();
            if (Category != null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم موجود "
                    : "The Category Created before ";
                return Ok(_baseResponse);
            }

            try
            {
                await _unitOfWork.Catagories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في اضافة القسم "
                    : "Error In Add Category ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم اضافة القسم بنجاح"
                : "The Category Has Been Added Successfully";

            return Ok(_baseResponse);

        }

        //--------------------------------------------------------------------------------------------------------
        // Get All Category
        [HttpGet("AllCategory")]
        public async Task<ActionResult<BaseResponse>> GetAllCategory([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Category =await _unitOfWork.Catagories.FindByQuery(s => s.IsDeleted == false).Select(s => new { s.CategoryId, s.CategoryName } ).ToListAsync();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category not Exits ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Category;
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Get Category
        [HttpGet("CategoryById")]
        public async Task<ActionResult<BaseResponse>> GetCategory([FromHeader] string lang, [FromForm] string Id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var Category = await _unitOfWork.Catagories.FindByQuery(s => s.IsDeleted == false && s.CategoryId == Id).Select(s => new { s.CategoryId, s.CategoryName }).ToListAsync();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category not Exits ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Category;
            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Update Category
        [HttpPut("UpdateCategory")]
        public async Task<ActionResult<BaseResponse>> UpdateCategory([FromHeader] string lang, [FromForm] Category category)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Admin)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  ادمن"
                    : "The User Not Admin ";
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

            var Category = _unitOfWork.Catagories.FindByQuery(s => s.CategoryId == category.CategoryId).FirstOrDefault();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category Not Exits ";
                return Ok(_baseResponse);
            }

            Category.CategoryName = category.CategoryName;
            try
            {
                _unitOfWork.Catagories.Update(Category);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في تعديل القسم "
                    : "Error In edit Category ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم تعديل القسم بنجاح"
                : "The Category Has Been Updated Successfully";

            return Ok(_baseResponse);

        }

        //--------------------------------------------------------------------------------------------------------
        // Delete Category
        [HttpDelete("DeleteCategory")]
        public async Task<ActionResult<BaseResponse>> DeleteCategory([FromHeader] string lang, [FromForm] string Id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Admin)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  ادمن"
                    : "The User Not Admin ";
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

            var Category = _unitOfWork.Catagories.FindByQuery(s => s.CategoryId == Id).FirstOrDefault();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category Not Exits ";
                return Ok(_baseResponse);
            }

            Category.IsDeleted = true;
            try
            {
                _unitOfWork.Catagories.Update(Category);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في حذف القسم "
                    : "Error In Delete Category ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم حذف القسم بنجاح"
                : "The Category Has Been Deleted Successfully";

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Create Post 
        [HttpPost("AddPost")]
        public async Task<ActionResult<BaseResponse>> AddPost([FromHeader] string lang, [FromForm] PostDTO postDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Youth)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  شاب"
                    : "The User Not Youth ";
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

            var Category = _unitOfWork.Catagories.FindByQuery(s => s.CategoryId == postDTO.CatagoryId).FirstOrDefault();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category Not Exist ";
                return Ok(_baseResponse);
            }
            if (postDTO.Attachment.Count()!=0)
                try
                {
                    foreach (var PostImg in postDTO.Attachment)
                    {
                        string img = await _fileHandling.UploadFile(PostImg, "Post");
                        postDTO.AttachmentUrls.Add(img);
                    }
                }
                catch
                {
                    _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                    _baseResponse.ErrorMessage = lang == "ar"
                        ? "خطأ في رفع الملفات "
                        : "Error In Upload Attachments ";
                    return Ok(_baseResponse);
                }

            var Post = new Post
            {
                UserId = _user.Id,
                PostText = postDTO.PostText,
                AttachmentUrls = postDTO.AttachmentUrls,
                Attachment = postDTO.Attachment,
                AttachmentUrl = ConvertListToString(postDTO.AttachmentUrls),
                CatagoryId = postDTO.CatagoryId
            };

            try
            {
                await _unitOfWork.Posts.AddAsync(Post);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في اضافة المنشور "
                    : "Error In Add Post ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم اضافة المنشور بنجاح"
                : "The Post Has Been Added Successfully";

            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET all Post
        [HttpGet("AllPost")]
        public async Task<ActionResult<BaseResponse>> getAllPost([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var Posts = await _unitOfWork.Posts.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Category.IsDeleted == false,
                orderBy: s => s.OrderByDescending(Post => Post.CreatedOn))
                .Select(s => new
                {
                    s.PostId,
                    s.CreatedOn,
                    s.PostText,
                    Category= new
                    {
                        s.Category.CategoryName,
                        s.Category.IsDeleted
                    },
                    Reacts= s.Reacts.Where(a => a.PostId == s.PostId).ToList(),
                    Comments = s.Comments.Where(a => a.PostId == s.PostId).Select(s => new
                    {
                        s.CommentId,
                        s.CreatedOn,
                        s.Text,
                        User = new
                        {
                            s.User.Id,
                            s.User.FirstName,
                            s.User.LastName,
                            s.User.Email,
                        }
                    }).ToList(),
                    Attachments = ConvertStringToList(s.AttachmentUrl),
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Posts;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET Post For User
        [HttpGet("PostForUser")]
        public async Task<ActionResult<BaseResponse>> getPostUser([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var Posts = await _unitOfWork.Posts.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Category.IsDeleted == false && s.UserId == _user.Id,
                orderBy: s => s.OrderByDescending(Post => Post.CreatedOn))
                .Select(s => new
                {
                    s.PostId,
                    s.CreatedOn,
                    s.PostText,
                    Category = new
                    {
                        s.Category.CategoryName,
                        s.Category.IsDeleted
                    },
                    Reacts = s.Reacts.Select(a => a.PostId == s.PostId).ToList(),
                    Comments = s.Comments.Select(a => a.PostId == s.PostId).ToList(),
                    Attachments = ConvertStringToList(s.AttachmentUrl),
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Posts;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Put Post
        [HttpPut("UpdatePost")]
        public async Task<ActionResult<BaseResponse>> UpdatePost([FromHeader] string lang, [FromForm] PostDTO postDTO)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Youth)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  شاب"
                    : "The User Not Youth ";
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

            var post = await _unitOfWork.Posts.FindByQuery(s =>
                s.PostId == postDTO.Id && s.UserId == _user.Id && s.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (post == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "المنشور غير موجودة "
                    : "The Post Not Exist ";
                return Ok(_baseResponse);
            }

            var Category = _unitOfWork.Catagories.FindByQuery(s => s.CategoryId == postDTO.CatagoryId).FirstOrDefault();
            if (Category == null)
            {
                _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "القسم غير موجود "
                    : "The Category Not Exist ";
                return Ok(_baseResponse);
            }

            if (postDTO.Attachment.Count() != 0)
                try
                {
                    foreach (var PostImg in postDTO.Attachment)
                    {
                        postDTO.AttachmentUrls.Add(await _fileHandling.UploadFile(PostImg, "Post"));
                    }
                }
                catch
                {
                    _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                    _baseResponse.ErrorMessage = lang == "ar"
                        ? "خطأ في رفع الملفات "
                        : "Error In Upload Attachments ";
                    return Ok(_baseResponse);
                }

            post.PostText = postDTO.PostText;
            post.CatagoryId = postDTO.CatagoryId;
            post.AttachmentUrl = ConvertListToString(postDTO.AttachmentUrls);

            _unitOfWork.Posts.Update(post);
            await _unitOfWork.SaveChangesAsync();

            #region  return data Error 
            #endregion


            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم تعديل المنشور بنجاح"
                : "The Post Has Been updated Successfully";
            /* _baseResponse.Data = newData;*/

            return Ok(_baseResponse);

        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Delete Post
        [HttpDelete("DeletePost")]
        public async Task<ActionResult<BaseResponse>> DeletePost([FromHeader] string lang, [FromForm] string id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            if (_user.UserType != UserType.Youth)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب ليس  شاب"
                    : "The User Not Youth ";
                return Ok(_baseResponse);
            }

            var post = await _unitOfWork.Posts.FindByQuery(s =>
                s.PostId == id && s.UserId == _user.Id && s.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (post == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "الخدمة غير موجودة "
                    : "The Service Not Exist ";
                return Ok(_baseResponse);
            }

            post.IsDeleted = true;

            _unitOfWork.Posts.Update(post);
            await _unitOfWork.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم حذف المنشور بنجاح"
                : "The Post Has Been Deleted Successfully";
            _baseResponse.Data = new { };

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Create Comment 
        [HttpPost("AddComment")]
        public async Task<ActionResult<BaseResponse>> AddComment([FromHeader] string lang, [FromForm] CommentPost commentPost)
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

            var Comment = new CommentPost
            {
                UserId = _user.Id,
                Text = commentPost.Text,
                PostId = commentPost.PostId,
            };

            try
            {
                await _unitOfWork.Comments.AddAsync(Comment);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في اضافة التعليق  "
                    : "Error In Add comment ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم اضافة التعليق بنجاح"
                : "The Comment Has Been Added Successfully";

            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET all Comment
        [HttpGet("AllComment")]
        public async Task<ActionResult<BaseResponse>> getAllComment([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var Comments = await _unitOfWork.Comments.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false,
                orderBy: s => s.OrderByDescending(Comments => Comments.CreatedOn))
                .Select(s => new
                {
                    s.CommentId,
                    s.CreatedOn,
                    s.Text,
                    User = new
                    {
                        Id=s.UserId,
                        FirstName=s.User.FirstName,
                        LastName=s.User.LastName,
                        Email=s.User.Email,
                    },
                    Post = new
                    {
                        Id = s.Post.PostId,
                        TextPost = s.Post.PostText,
                        AttachmentPost = ConvertStringToList(s.Post.AttachmentUrl),
                        IsDeleted = s.Post.IsDeleted,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Comments;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET all Comment
        [HttpGet("AllCommentToPost")]
        public async Task<ActionResult<BaseResponse>> getAllCommentToPost([FromHeader] string lang, [FromForm] string IdPost)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var Comments = await _unitOfWork.Comments.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false && s.PostId == IdPost,
                orderBy: s => s.OrderByDescending(Comments => Comments.CreatedOn))
                .Select(s => new
                {
                    s.CommentId,
                    s.CreatedOn,
                    s.Text,
                    User = new
                    {
                        Id = s.UserId,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                    },
                    Post = new
                    {
                        Id = s.Post.PostId,
                        TextPost = s.Post.PostText,
                        AttachmentPost = ConvertStringToList(s.Post.AttachmentUrl),
                        IsDeleted = s.Post.IsDeleted,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Comments;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET Comment For User
        [HttpGet("CommentForUser")]
        public async Task<ActionResult<BaseResponse>> getCommentUser([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var Comments = await _unitOfWork.Comments.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false && s.UserId == _user.Id,
                orderBy: s => s.OrderByDescending(Comments => Comments.CreatedOn))
                .Select(s => new
                {
                    s.CommentId,
                    s.CreatedOn,
                    s.Text,
                    User = new
                    {
                        Id = s.UserId,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = Comments;
            return Ok(_baseResponse);
        }


        //---------------------------------------------------------------------------------------------------------------------------------
        //Put Commment
        [HttpPut("UpdateComment")]
        public async Task<ActionResult<BaseResponse>> UpdateComment([FromHeader] string lang, [FromForm] CommentPost commentPost)
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
                _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
                _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
                _baseResponse.Data = new
                {
                    message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                };
                return Ok(_baseResponse);
            }

            var comment = await _unitOfWork.Comments.FindByQuery(s =>
                s.CommentId == commentPost.CommentId && s.UserId == _user.Id && s.IsDeleted == false && s.Post.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (comment == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "التعليق غير موجودة "
                    : "The Comment Not Exist ";
                return Ok(_baseResponse);
            }

            comment.Text = commentPost.Text;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            #region  return data Error 
            #endregion


            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم تعديل التعليق بنجاح"
                : "The Comment Has Been updated Successfully";
            /* _baseResponse.Data = newData;*/

            return Ok(_baseResponse);

        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Delete Comment
        [HttpDelete("DeleteComment")]
        public async Task<ActionResult<BaseResponse>> DeleteComment([FromHeader] string lang, [FromForm] string id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var comment = await _unitOfWork.Comments.FindByQuery(s =>
                s.UserId == _user.Id && s.IsDeleted == false && s.Post.IsDeleted == false && s.CommentId == id && s.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (comment == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "التعليق غير موجودة "
                    : "The comment Not Exist ";
                return Ok(_baseResponse);
            }

            comment.IsDeleted = true;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم حذف التعليق بنجاح"
                : "The Comment Has Been Deleted Successfully";
            _baseResponse.Data = new { };

            return Ok(_baseResponse);
        }

        //--------------------------------------------------------------------------------------------------------
        // Create React 
        [HttpPost("AddReact")]
        public async Task<ActionResult<BaseResponse>> AddReact([FromHeader] string lang, [FromForm] ReactPost ReactPost)
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

            var React = new ReactPost
            {
                UserId = _user.Id,
                PostId = ReactPost.PostId,
            };

            try
            {
                await _unitOfWork.Reacts.AddAsync(React);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في اضافة الاعجاب  "
                    : "Error In Add React ";
                return Ok(_baseResponse);
            }

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم اضافة الاعجاب بنجاح"
                : "The React Has Been Added Successfully";

            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET all React
        [HttpGet("AllReact")]
        public async Task<ActionResult<BaseResponse>> getAllReact([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var React = await _unitOfWork.Reacts.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false)
                .Select(s => new
                {
                    s.ReactId,
                    User = new
                    {
                        Id = s.UserId,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = React;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET React For User
        [HttpGet("ReactForUser")]
        public async Task<ActionResult<BaseResponse>> getReactUser([FromHeader] string lang)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var React = await _unitOfWork.Reacts.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false && s.UserId == _user.Id)
                .Select(s => new
                {
                    s.ReactId,
                    Post= new
                    {
                        Id = s.PostId,
                        Attachments = ConvertStringToList(s.Post.AttachmentUrl),
                        Text = s.Post.PostText
                    },
                    User = new
                    {
                        Id = s.UserId,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = React;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //GET React For User
        [HttpGet("ReactForPost")]
        public async Task<ActionResult<BaseResponse>> getReactPost([FromHeader] string lang , [FromForm] string Id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = (lang == "ar")
                    ? "هذا الحساب غير موجود   "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }
            var React = await _unitOfWork.Reacts.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Post.IsDeleted == false && s.UserId == _user.Id && s.ReactId == Id)
                .Select(s => new
                {
                    s.ReactId,
                    Post = new
                    {
                        Id = s.PostId,
                        Attachments = ConvertStringToList(s.Post.AttachmentUrl),
                        Text = s.Post.PostText
                    },
                    User = new
                    {
                        Id = s.UserId,
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Email = s.User.Email,
                    }
                }).ToListAsync();
            _baseResponse.ErrorCode = 0;
            _baseResponse.Data = React;
            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Delete React
        [HttpDelete("DeleteReact")]
        public async Task<ActionResult<BaseResponse>> DeleteReact([FromHeader] string lang, [FromForm] string id)
        {
            if (_user == null)
            {
                _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "هذا الحساب غير موجود "
                    : "The User Not Exist ";
                return Ok(_baseResponse);
            }

            var React = await _unitOfWork.Reacts.FindByQuery(s =>
                s.ReactId == id && s.UserId == _user.Id && s.IsDeleted == false && s.Post.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (React == null)
            {
                _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "التعليق غير موجودة "
                    : "The comment Not Exist ";
                return Ok(_baseResponse);
            }

            React.IsDeleted = true;

            _unitOfWork.Reacts.Update(React);
            await _unitOfWork.SaveChangesAsync();

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم حذف الاعجاب بنجاح"
                : "The React Has Been Deleted Successfully";
            _baseResponse.Data = new { };

            return Ok(_baseResponse);
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        //Function
        static string ConvertListToString(List<string> list)
        {
            return string.Join(",", list);
        }

        static List<string> ConvertStringToList(string inputString)
        {
            // Split the input string by commas and convert it to a List<string>
            return inputString.Split(',').ToList();
        }
    }
}

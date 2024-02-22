using Investor.Core.Entity.ApplicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using Microsoft.AspNetCore.Http;

namespace Investor.Core.Entity.PostData
{
    public class Post
    {
        public string PostId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "حالة المنشور")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "النص")]
        [Required(ErrorMessage = "يجب أدخال النص "), StringLength(int.MaxValue), MinLength(2, ErrorMessage = "يجب أن يكون النص أكبر من 2 حروف")]
        public string PostText { get; set; }

        [Display(Name = " الملفات ")]
        public string AttachmentUrl { get; set; }

        [Display(Name = "تاريخ النشر")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        //-----------------------------------------

        [ForeignKey("User")]
        [Display(Name = "اسم الناشر ")]
        public string UserId { get; set; }

        [Display(Name = "اسم الناشر ")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "اسم القسم ")]
        public string CatagoryId { get; set; }

        [Display(Name = "اسم القسم ")]
        public virtual Category Category { get; set; }
        //------------------------------------------
        [NotMapped]
        [Display(Name = " الملفات  ")]
        public List<IFormFile> Attachment { get; set; }

        [NotMapped]
        public List<string> AttachmentUrls { get; set; }
        //-----------------------------------------
        public IEnumerable<ReactPost> Reacts { get; set; } = new List<ReactPost>();
        public IEnumerable<CommentPost> Comments { get; set; } = new List<CommentPost>();
    }
}

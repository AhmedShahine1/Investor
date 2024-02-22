using Investor.Core.Entity.ApplicationData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.Entity.PostData
{
    public class Comment
    {

        [Display(Name = "اسم المعلق "),Required, MinLength(2,ErrorMessage ="يجب ادخال 2 حروف علي الاقل"), MaxLength(int.MaxValue)]
        public string Text {  get; set; }

        [Display(Name = "حالة التعليق")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "تاريخ النشر")]
        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        [Display(Name = "اسم المعلق ")]
        public string UserId { get; set; }

        [Display(Name = "اسم المعلق ")]
        public virtual ApplicationUser User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.Entity.PostData
{
    public class Category
    {
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "القسم")]
        [Required(ErrorMessage = "يجب أدخال اسم القسم "), StringLength(int.MaxValue), MinLength(2, ErrorMessage = "يجب أن يكون اسم القسم أكبر من 2 حروف")]
        public string CategoryName { get; set; }

        [Display(Name = "حالة القسم")]
        public bool IsDeleted { get; set; } = false;

    }
}

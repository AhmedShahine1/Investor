using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Investor.Core.Entity.ApplicationData;

namespace Investor.Core.Entity.EvaluationData
{
    public class Evaluation : BaseEntity
    {
        [ForeignKey("User")]
        [Display(Name = "اسم العميل ")]
        public string UserId { get; set; }
        [Display(Name = "اسم العميل ")]
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "التقييم مطلوب ")]
        [Display(Name = "التقييم ")]
        [Range(1, 5, ErrorMessage = "التقييم يجب ان يكون بين 1 و 5 ")]
        public int NumberOfStars { get; set; }

    }
}

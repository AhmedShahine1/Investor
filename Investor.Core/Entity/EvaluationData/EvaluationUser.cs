using Investor.Core.Entity.ApplicationData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investor.Core.Entity.EvaluationData;

public class EvaluationUser : Evaluation
{

    [ForeignKey("TargetUser")]
    [Display(Name = "اسم المقييم ")]
    public string TargetUserId { get; set; }
    [Display(Name = "اسم المقييم ")]
    public virtual ApplicationUser TargetUser { get; set; }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.DTO.EntityDto;

public class UserEvaluationDto
{
    [Required(ErrorMessage = "التقييم مطلوب ")]
    [Display(Name = "التقييم ")]
    [Range(1, 5, ErrorMessage = "التقييم يجب ان يكون بين 1 و 5 ")]
    public int NumberOfStars { get; set; }

    [Required(ErrorMessage = "اسم المقييم مطلوب ")]
    [Display(Name = "اسم المقييم ")]
    public string TargetUserId { get; set; }
        
}
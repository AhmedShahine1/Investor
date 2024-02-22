using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.DTO.EntityDTO
{
    public class ConnectionDTO
    {
        [Required(ErrorMessage = "اسم الصديق مطلوب ")]
        [Display(Name = "اسم الصديق ")]
        public string TargetUserId { get; set; }

        public bool Agree { get; set; }
    }
}

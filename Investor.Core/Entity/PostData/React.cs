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
    public class React
    {
        public string ReactId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "حالة الرياكت")]
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("User")]
        [Display(Name = "اسم المعجب ")]
        public string UserId { get; set; }

        [Display(Name = "اسم المعلق ")]
        public virtual ApplicationUser User { get; set; }
    }
}

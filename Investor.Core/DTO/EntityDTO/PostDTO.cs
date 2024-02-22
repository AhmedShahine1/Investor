using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Investor.Core.Helpers;

namespace Investor.Core.DTO.EntityDTO
{
    public class PostDTO
    {
        public string Id { get; set; }

        [Display(Name = "النص")]
        [Required(ErrorMessage = "يجب أدخال النص "), StringLength(int.MaxValue), MinLength(2, ErrorMessage = "يجب أن يكون النص أكبر من 2 حروف")]
        [ExclusiveField("Attachment", ErrorMessage = "Either Text or Attachment should be entered, not both.")]
        public string PostText { get; set; }

        //---------------------------------------
        [Display(Name = " الملفات  ")]
        [ExclusiveField("PostText", ErrorMessage = "Either Post or Attachment should be entered, not both.")]
        public List<IFormFile> Attachment { get; set; }

        public List<string> AttachmentUrls { get; set; } = new List<string>();

        //---------------------------------------
        [Display(Name = " القسم  ")]
        [Required(ErrorMessage ="يجب اختيار قسم")]
        public string CatagoryId { get; set; }


    }
}

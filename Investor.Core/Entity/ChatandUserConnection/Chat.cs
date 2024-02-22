using Investor.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.Entity.ChatandUserConnection
{
    public class Chat : BaseEntity
    {
        public string ChatId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "الرساله "), StringLength(int.MaxValue)]
        public string Message { get; set; }

        [ForeignKey("SendUser")]
        public string SendUserId { get; set; }

        public virtual ApplicationUser SendUser { get; set; }

        [ForeignKey("ReceiveUser")]
        public string ReceiveUserId { get; set; }

        public virtual ApplicationUser ReceiveUser { get; set; }

        [NotMapped]
        public List<IFormFile> Attachment { get; set; }

        [NotMapped]
        public List<string> AttachmentUrls { get; set; }

        public string AttachmentUrl { get; set; }

        public bool IsRead { get; set; } = false;
    }
}

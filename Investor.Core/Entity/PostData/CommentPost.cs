using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.Entity.PostData
{
    public class CommentPost : Comment
    {
        public string CommentId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Post")]
        [Required, Display(Name = "المنشور")]
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}

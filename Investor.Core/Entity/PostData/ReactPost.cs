using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.Core.Entity.PostData
{
    public class ReactPost : React
    {
        [ForeignKey("Post")]
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}

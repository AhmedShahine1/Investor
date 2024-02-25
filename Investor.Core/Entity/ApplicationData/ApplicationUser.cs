using System.ComponentModel.DataAnnotations.Schema;
using Investor.Core.Entity.ChatandUserConnection;
using Investor.Core.Entity.ConnectionData;
using Investor.Core.Entity.EvaluationData;
using Investor.Core.Entity.PostData;
using Investor.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Investor.Core.Entity.ApplicationData;

public class ApplicationUser : IdentityUser
{

    public bool IsAdmin { get; set; } = false;//if true, user is admin
    public bool Status { get; set; } = true;//true=active,false=disactive
    //-------------------------------------------------------------------
    public UserType UserType { get; set; } // 0 = Admin,1 = Investor ,2 = Youth
    public string Qualification { get; set; } // only for center and freelancer
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string UserImgUrl { get; set; }
    public string Job { get; set; }
    public int Age { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    //------------------------------------------------------------------------------------------------
    public IEnumerable<Post> Posts { get; set; } = new List<Post>(); // for all users
    public IEnumerable<EvaluationUser> EvaluationUsers { get; set; } = new List<EvaluationUser>(); // for all users
    public IEnumerable<Connection> connections { get; set; } = new List<Connection>(); // for all users
    public IEnumerable<Chat> chats { get; set; } = new List<Chat>();

    //------------------------------------------------------------------------------------------------
    [NotMapped]
    public IFormFile UserImgFile { get; set; }

   
}
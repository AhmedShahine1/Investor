using Investor.Core.Entity.ApplicationData;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investor.Core.Entity.ChatAndNotification;

public class UserConnection
{
    public string UserConnectionId { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("User")]
    public string UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    public bool Connection { get; set; } = true; // if Connection is true user is active now if no user not active 

    public DateTime ConnectionTime { get; set; } = DateTime.Now;
}
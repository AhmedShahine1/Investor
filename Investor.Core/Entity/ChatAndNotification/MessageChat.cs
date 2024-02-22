using Investor.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investor.Core.Entity.ChatAndNotification;

public class MessageChat
{
    public string ChatId { get; set; } = Guid.NewGuid().ToString();

    [Display(Name ="الرساله "), StringLength(int.MaxValue)]
    public string Message { get; set; }

    public DateTime MessageDate { get; set; } = DateTime.Now;

    [ForeignKey("SendUser")]
    public string SendUserId { get; set; }

    public virtual ApplicationUser SendUser { get; set; }

    [ForeignKey("ReceiveUser")]
    public string ReceiveUserId { get; set; }

    public virtual ApplicationUser ReceiveUser { get; set; }

    [NotMapped]
    public IFormFile Img { get; set; }

    public string ImgUrl { get; set; }

    public bool IsRead { get; set; }
}
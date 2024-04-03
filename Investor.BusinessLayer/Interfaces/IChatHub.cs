using Investor.Core.Entity.ChatandUserConnection;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.BusinessLayer.Interfaces
{
    public interface IChatHub
    {
        Task SendMessage(string user, string message);
        void SendChatMessage(Chat chat);
    }
}

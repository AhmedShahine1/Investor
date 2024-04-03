﻿using Microsoft.AspNetCore.SignalR;
using Investor.Core.Entity.ApplicationData;
using Investor.RepositoryLayer.Interfaces;
using Investor.BusinessLayer.Interfaces;
using Investor.Core.DTO;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Investor.Core.Helpers;
using Investor.Core.DTO.EntityDTO;
using Investor.Core.Entity.ChatandUserConnection;
using Microsoft.AspNetCore.Http.HttpResults;


namespace Investor.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileHandling _fileHandling;
        private readonly BaseResponse _baseResponse;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(IUnitOfWork unitOfWork, IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _fileHandling = fileHandling;
            _baseResponse = new BaseResponse();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(ChatDTO chatDTO)
        {
            if (chatDTO == null || string.IsNullOrEmpty(chatDTO.ReceiveUserId) || (string.IsNullOrEmpty(chatDTO.Message)  && chatDTO.Attachment == null))
            {
                return;
            }
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            var userId = JWT(accessToken);

            if (userId != null)
            {
                var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId && s.Status)
                              .FirstOrDefault();
                var ReceivedUser = _unitOfWork.Users.FindByQuery(s => s.Id == chatDTO.ReceiveUserId && s.Status)
                                              .FirstOrDefault();
                if(user != null&& ReceivedUser != null)
                {
                    if (chatDTO.Attachment != null)
                        try
                        {
                            foreach (var ChatImg in chatDTO.Attachment)
                            {
                                string img = await _fileHandling.UploadFile(ChatImg, "Chat");
                                chatDTO.AttachmentUrls.Add(img);
                            }
                        }
                        catch
                        {
                            await Clients.Caller.SendAsync("Error", "Error in upload image");
                        }

                    var Chat = new Chat
                    {
                        Message = chatDTO.Message,
                        SendUserId = user.Id,
                        ReceiveUserId = ReceivedUser.Id,
                        AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : null,
                        IsRead = false
                    };
                    try
                    {
                        _unitOfWork.Chats.Add(Chat);
                        await _unitOfWork.SaveChangesAsync();
                        await Clients.Caller.SendAsync("Message", new
                        {
                            Message= chatDTO.Message,
                            SendUserId = user.Id,
                            ReceiveUserId = ReceivedUser.Id,
                            AttachmentUrl = (chatDTO.AttachmentUrls.Count() != 0) ? ConvertListToString(chatDTO.AttachmentUrls) : null,
                            IsRead = false,
                            CreatedAt= Chat.CreatedAt
                        });
                    }
                    catch
                    {
                        await Clients.Caller.SendAsync("Error", "Error In send message");
                    }
                }
                await Clients.Caller.SendAsync("Error", "Error In send message");
            }
            await Clients.Caller.SendAsync("Error", "Error In send message");
        }

        public override async Task OnConnectedAsync()
        {
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            var ReceivedId = Context.GetHttpContext().Request.Query["ReceivedId"].ToString();
            var userId = JWT(accessToken);
            if (userId != null && ReceivedId != null)
            {
                var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId && s.Status)
                                              .FirstOrDefault();
                var ReceivedUser= _unitOfWork.Users.FindByQuery(s => s.Id == ReceivedId && s.Status)
                                              .FirstOrDefault();
                var ReadMessages = await _unitOfWork.Chats.FindByQuery(s => (s.SendUserId == ReceivedId && s.ReceiveUserId == user.Id)).ToListAsync();

                foreach (var message in ReadMessages)
                {
                    message.IsRead = true;
                    try
                    {
                        _unitOfWork.Chats.Update(message);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    catch
                    {
                        return ;
                    }

                }
                var Messages = await _unitOfWork.Chats.FindByQuery(s => (s.SendUserId == user.Id && s.ReceiveUserId == ReceivedId) || (s.ReceiveUserId == user.Id && s.SendUserId == ReceivedId)).Select(x => new
                {
                    x.ChatId,
                    x.Message,
                    SenderUser = new
                    {
                        x.SendUser.Id,
                        x.SendUser.FirstName,
                        x.SendUser.LastName,
                        x.SendUser.Email,
                    },
                    ReceiveUser = new
                    {
                        x.ReceiveUser.Id,
                        x.ReceiveUser.FirstName,
                        x.ReceiveUser.LastName,
                        x.ReceiveUser.Email,
                    },
                    SendTime = (x.IsUpdated) ? x.UpdatedAt : x.CreatedAt,
                    x.IsRead,
                    x.IsDeleted,
                    x.IsUpdated,
                    Attachment = ConvertStringToList(x.AttachmentUrl)
                }).ToListAsync();

                await Clients.Caller.SendAsync("Message", Messages);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        //--------------------------------------------------------------------------------------

        public string JWT(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                // Invalid token format
                return null;
            }

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid").Value;
            return userId;
        }

        static List<string> ConvertStringToList(string inputString)
        {
            // Split the input string by commas and convert it to a List<string>
            return inputString.Split(',').ToList();
        }

        static string ConvertListToString(List<string> list)
        {
            return string.Join(",", list);
        }


    }
}
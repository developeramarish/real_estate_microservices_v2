using Chat.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        private UserInfoInMemory _userInfoInMemory;

        public MainHub(UserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        public async Task Leave()
        {
            _userInfoInMemory.Remove(Context.User.Identity.Name);
            await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
                   "UserLeft",
                   Context.User.Identity.Name
                   );
        }

        public async Task Join()
        {
            if (!_userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId))
            {
                // new user

                var list = _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name).ToList();
                await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
                    "NewOnlineUser",
                    _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
                    );
            }
            else
            {
                // existing user joined again

            }

            await Clients.Client(Context.ConnectionId).SendAsync(
                "Joined",
                _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
                );

            await Clients.Client(Context.ConnectionId).SendAsync(
                "OnlineUsers",
                _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name)
            );
        }

        public Task SendDirectMessage(string message, string targetUserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetUserName);
            return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDM", message, userInfoSender);
        }
        //public Task SendPrivateMessage(string user, string message)
        //{
        //    return Clients.User(user).SendAsync("ReceiveMessage", message);
        //}

        //public void SendPrivateMessage2(string receiverId, string message)
        //{
        //    var to = ConnectedUsers.SingleOrDefault(x => x.ConnectionId == receiverId);
        //    var from = ConnectedUsers.SingleOrDefault(x => x.ConnectionId == Context.ConnectionId);

        //    if (to != null && from != null)
        //    {
        //        Clients.Client(receiverId).sendPrivateMessage(Context.ConnectionId, from.UserName, message);
        //        Clients.Caller.sendPrivateMessage(receiverId, from.UserName, message);
        //    }
        //}

        //public async Task AddToGroup(string groupName)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        //    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        //}

        //public async Task RemoveFromGroup(string groupName)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        //    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        //}
    }
}

using Chat.Chat;
using Chat.Domain.Entities;
using Chat.Infrastructure.IRepositories.Interfaces;
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
        // private UserInfoInMemory _userInfoInMemory;
        private readonly IUserRepository _userRepository;
        private readonly IChatRoomRepository _chatRoomRepository;

        public MainHub(IUserRepository userRepository, IChatRoomRepository chatRoomRepository, UserInfoInMemory userInfoInMemory)
        {
            // _userInfoInMemory = userInfoInMemory;
            _userRepository = userRepository;
            _chatRoomRepository = chatRoomRepository;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                //var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
                var user = await _userRepository.GetByMySqlIdAsync(IdentityName);
                //var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
                //userViewModel.Device = GetDevice();
                //userViewModel.CurrentRoom = "";
                if(user == null)
                {
                    await Clients.Caller.SendAsync("onError", "No user");
                }

                if (user.SignalRConnectionIds.Any())
                {
                    user.AddConnectionId(Context.ConnectionId);
                    await _userRepository.UpdateConnectionIdsAsync(user);
                }

                await Clients.Caller.SendAsync("getProfileInfo", user);
            }
            catch (Exception ex)
            {
               await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            await base.OnConnectedAsync();
            //return result;
        }

        public async Task SendPrivateMessage(string receiverMySqlId, string message, string chatRoomId)
        {
                // Who is the sender;
                var sender = await _userRepository.GetByMySqlIdAsync(IdentityName);

                if (sender == null)
                {
                    await Clients.Caller.SendAsync("onError", "No user");
                }

                if (!sender.SignalRConnectionIds.Any())
                {
                    await Clients.Caller.SendAsync("onError", "Some issue. Shoud be connected first");
                }

                // who is the receiver
                var receiver = await _userRepository.GetByMySqlIdAsync(Int32.Parse(receiverMySqlId));

                var chatRoom = await _chatRoomRepository.GetByIdAsync(chatRoomId);

                chatRoom.AddMessage(new Message() { Created = DateTime.UtcNow, MessageText = message });

                await _chatRoomRepository.UpdateMessagesAsync(chatRoom);

                if (receiver != null && receiver.SignalRConnectionIds.Any())
                {
                    foreach(var conn in receiver.SignalRConnectionIds)
                    {
                        await Clients.Client(conn).SendAsync("newMessage", message);
                    }
                }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                //var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
                var user = await _userRepository.GetByMySqlIdAsync(IdentityName);
                //var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
                //userViewModel.Device = GetDevice();
                //userViewModel.CurrentRoom = "";
                if (user == null)
                {
                    await Clients.Caller.SendAsync("onError", "No user to disconnect");
                }

                user.RemoveConnectionId(Context.ConnectionId);
                await _userRepository.UpdateConnectionIdsAsync(user);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }

            await base.OnDisconnectedAsync(exception);
        }

        //public Task SendDirectMessage(string message, string targetUserName)
        //{
        //    var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
        //    var userInfoReciever = _userInfoInMemory.GetUserInfo(targetUserName);
        //    return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDM", message, userInfoSender);
        //}

        private int IdentityName
        {
            get { return Int32.Parse(Context.User.Identity.Name); }
        }

        //public async Task Leave()
        //{
        //    _userInfoInMemory.Remove(Context.User.Identity.Name);
        //    await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
        //           "UserLeft",
        //           Context.User.Identity.Name
        //           );
        //}

        //public async Task Join()
        //{
        //    if (!_userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId))
        //    {
        //        //var httpCtx1 = Context.GetHttpContext();
        //        //var mySqlId1 = httpCtx1.Request.Headers["mysqlid"].ToString();
        //        //// new user
        //        //var dfdf1 = 0;
        //        // var list = _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name).ToList();
        //        await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
        //            "NewOnlineUser",
        //            _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
        //            );
        //    }
        //    else
        //    {
        //        // existing user joined again

        //    }

        //    await Clients.Client(Context.ConnectionId).SendAsync(
        //        "Joined",
        //        _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
        //        );

        //    //var httpCtx = Context.GetHttpContext();
        //    //var mySqlId = httpCtx.Request.Headers["mysqlid"].ToString();
        //    //// new user
        //    //var dfdf = 0;

        //    await Clients.Client(Context.ConnectionId).SendAsync(
        //        "OnlineUsers",
        //        _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name)
        //    );
        //}


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

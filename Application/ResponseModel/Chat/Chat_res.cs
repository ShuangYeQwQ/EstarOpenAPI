using Application.ResponseModel.ServicePage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseModel.Chat
{
    public class Chat_res
    {
        //id
        public string Id { get; set; }
        //聊天名称
        public string Name { get; set; }
        //聊天头像
        public string ChatAvatar { get; set; }
        //最后一条消息
        public string LastMessage { get; set; }
        //最后消息时间
        public string LastTimeStamp { get; set; }
        //是否群聊
        public int IsGroupChat { get; set; }
        //当前用户未读消息数
        public int UnreadCount { get; set; }
        

        public List<ChatUser_res> ChatUser { get; set; }
    }
    public class ChatUser_res
    {
        //聊天id
        public string Id { get; set; }
        //聊天用户id
        public string Uid { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
        //聊天用户未读数
        public string UnreadMessageNumber { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class FetchMessageResponse
    {
        public int AddMsgCount { get; set; }
        public List<MessageResponse> AddMsgList { get; set; }
        public BaseResponse BaseResponse { get; set; }
        public int ContinueFlag { get; set; }
        public int DelContactCount { get; set; }
        //DelContactList
        public int ModChatRoomMemberCount { get; set; }
        //ModChatRoomMemberList
        public int ModContactCount { get; set; }
        public List<ModContact> ModContactList { get; set; }
        //Profile
        public string SKey { get; set; }
        public SyncKey SyncCheckKey { get; set; }
        public SyncKey SyncKey { get; set; }
    }
}

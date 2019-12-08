using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class SendMessageBody
    {
        public int Type { get; set; }

        public string Content { get; set; }

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public string LocalID { get; set; }

        public string ClientMsgId => LocalID;

        public string MediaId { get; set; }
    }
}

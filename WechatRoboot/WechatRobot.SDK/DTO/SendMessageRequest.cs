using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class SendMessageRequest
    {
        public BaseRequest BaseRequest { get; set; }
        public SendMessageBody Msg { get; set; }
        public int Scene { get; set; } = 0;
    }
}

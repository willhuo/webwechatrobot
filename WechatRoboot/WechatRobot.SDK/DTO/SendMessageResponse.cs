using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class SendMessageResponse
    {
        public BaseResponse BaseResponse { get; set; }
        public string LocalID { get; set; }
        public string MsgID { get; set; }
    }
}

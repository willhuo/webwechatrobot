using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class BatchContactResponse
    {
        public BaseResponse BaseResponse { get; set; }

        public int Count { get; set; }

        public List<ContactUser> ContactList { get; set; }
    }
}

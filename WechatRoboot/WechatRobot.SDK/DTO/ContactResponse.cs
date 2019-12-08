using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class ContactResponse
    {
        public BaseResponse BaseResponse { get; set; }

        public int MemberCount { get; set; }

        public int Seq { get; set; }

        public List<ContactUser> MemberList { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class ModContact
    {
        public string Alias { get; set; }
        public long AttrStatus { get; set; }
        public string ChatRoomOwner { get; set; }
        public string City { get; set; }
        public int ContactFlag { get; set; }
        public int ContactType { get; set; }
        public int HeadImgUpdateFlag { get; set; }
        public string HeadImgUrl { get; set; }
        public int HideInputBarFlag { get; set; }
        public string KeyWord { get; set; }
        public int MemberCount { get; set; }
        //MemberList[]
        public string NickName { get; set; }
        public string Province { get; set; }
        public string RemarkName { get; set; }
        public int Sex { get; set; }
        public string Signature { get; set; }
        public int SnsFlag { get; set; }
        public int Statues { get; set; }
        public string UserName { get; set; }
        public int VerifyFlag { get; set; }
    }
}

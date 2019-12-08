using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    /// <summary>
    /// 微信群联系人
    /// </summary>
    public class BatchUser
    {
        public long AttrStatus { get; set; }
        public string DisplayName { get; set; }
        public string KeyWord { get; set; }
        public int MemberStatus { get; set; }
        public string NickName { get; set; }
        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }
        public int Uin { get; set; }
        public string UserName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    /// <summary>
    /// 当前登录用户信息
    /// </summary>
    public class User
    {
        public int AppAccountFlag { get; set; }
        public int ContactFlag { get; set; }
        public int HeadImgFlag { get; set; }
        public string HeadImgUrl { get; set; }
        public string HeadImgBase64 { get; set; }
        public int HideInputBarFlag { get; set; }
        public string NickName { get; set; }
        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }
        public string RemarkName { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }
        public int Sex { get; set; }
        public string Signature { get; set; }
        public int SnsFlag { get; set; }
        public int StarFriend { get; set; }
        public long Uin { get; set; }
        public string UserName { get; set; }
        public int VerifyFlag { get; set; }
        public int WebWxPluginSwitch { get; set; }
    }
}

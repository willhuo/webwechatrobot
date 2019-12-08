using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class ContactUser
    {
        public string Alias { get; set; }
        public int AppAccountFlag { get; set; }
        public long AttrStatus { get; set; }
        public int ChatRoomId { get; set; }

        [DisplayName("城市")]
        public string City { get; set; }

        [DisplayName("类型3:好友 2：群")]
        public int ContactFlag { get; set; }

        public string DisplayName { get; set; }
        public string EncryChatRoomId { get; set; }
        public string HeadImgUrl { get; set; }
        public string HeadImgBase64 { get; set; }
        public int HideInputBarFlag { get; set; }
        public int IsOwner { get; set; }
        public string KeyWord { get; set; }
        public int MemberCount { get; set; }
        public List<BatchUser> MemberList { get; set; }

        [DisplayName("昵称")]
        public string NickName { get; set; }

        public int OwnerUin { get; set; }

        [DisplayName("省份")]
        public string Province { get; set; }

        public string PYInitial { get; set; }
        public string PYQuanPin { get; set; }

        [DisplayName("备注名")]
        public string RemarkName { get; set; }
        public string RemarkPYInitial { get; set; }
        public string RemarkPYQuanPin { get; set; }

        [DisplayName("性别")]
        public int Sex { get; set; }

        [DisplayName("性别中文")]
        public string SexCN
        {
            get
            {
                switch (Sex)
                {
                    case 0: return "女";
                    case 1: return "男";
                    default: return Sex.ToString();
                }
            }
        }

        [DisplayName("签名")]
        public string Signature { get; set; }
        public int SnsFlag { get; set; }
        public int StarFriend { get; set; }
        public int Statues { get; set; }
        public long Uin { get; set; }
        public int UniFriend { get; set; }

        [DisplayName("用户名")]
        public string UserName { get; set; }

        [DisplayName("类型")]
        public int VerifyFlag { get; set; }

        [DisplayName("类型中文")]
        public string VerifyFlagCN
        {
            get
            {
                switch (VerifyFlag)
                {
                    case 0: return "好友";
                    case 8: return "订阅号";
                    case 24: return "公众号";
                    case 29: return "增强公众号";
                    default: return VerifyFlag.ToString();
                }
            }
        }
    }
}

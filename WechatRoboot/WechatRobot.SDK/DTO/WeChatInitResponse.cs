using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WechatRobot.SDK.DTO
{
    public class WeChatInitResponse
    {
        public BaseResponse BaseResponse { get; set; }

        /// <summary>
        /// 应该是微信群相关的UserName
        /// </summary>
        public string ChatSet { get; set; }

        public string[] ChatSetArray
        {
            get
            {
                if (string.IsNullOrEmpty(ChatSet))
                {
                    return default(string[]);
                }
                else
                {
                    var list = new List<string>();
                    var mas = Regex.Matches(ChatSet, "@[^,]+");
                    foreach (Match ma in mas)
                    {
                        list.Add(ma.ToString());
                    }
                    return list.ToArray();
                }
            }
        }

        public int ClickReportInterval { get; set; }
        public int ClientVersion { get; set; }
        public List<ContactUser> ContactList { get; set; }
        public int Count { get; set; }
        public int GrayScale { get; set; }
        public int InviteStartCount { get; set; }
        public int MPSubscribeMsgCount { get; set; }        
        public List<MPSubscribeMsg> MPSubscribeMsgList { get; set; }
        public string SKey { get; set; }
        public SyncKey SyncKey { get; set; }
        public long SystemTime { get; set; }

        /// <summary>
        /// 当前登录用户信息
        /// </summary>
        public User User { get; set; }
    }
}

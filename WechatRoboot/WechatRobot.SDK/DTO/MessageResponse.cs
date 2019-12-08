using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WechatRobot.SDK.DTO
{
    public class MessageResponse
    {
        public AppInfo AppInfo { get; set; }
        public int AppMsgType { get; set; }
        public string Content { get; set; }
        public long CreateTime { get; set; }
        public string EncryFileName { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public int ForwardFlag { get; set; }
        public string FromUserName { get; set; }
        public int HasProductId { get; set; }
        public int ImgHeight { get; set; }
        public int ImgStatus { get; set; }
        public int ImgWidth { get; set; }
        public string MediaId { get; set; }
        public string MsgId { get; set; }
        public int MsgType { get; set; }
        public long NewMsgId { get; set; }
        public string OriContent { get; set; }
        public long PlayLength { get; set; }
        public RecommendInfo RecommendInfo { get; set; }
        public int Status { get; set; }
        public int StatusNotifyCode { get; set; }
        public string StatusNotifyUserName { get; set; }
        public string[] StatusNotifyUserNameArray
        {
            get
            {
                if(string.IsNullOrEmpty(StatusNotifyUserName))
                {
                    return default(string[]);
                }
                else
                {
                    var list = new List<string>();
                    var mas = Regex.Matches(StatusNotifyUserName, "@[^,]+");
                    foreach(Match ma in mas)
                    {
                        list.Add(ma.ToString());
                    }
                    return list.ToArray();
                }                
            }
        }
        public int SubMsgType { get; set; }
        public string Ticket { get; set; }
        public string ToUserName { get; set; }
        public string Url { get; set; }
        public long VoiceLength { get; set; }    
    }

    public class AppInfo
    {
        public string AppID { get; set; }
        public int Type { get; set; }
    }

    public class RecommendInfo
    {
        public string Alias { get; set; }
        public int AttrStatus { get; set; }
        public string City { get; set; }
        public string Content { get; set; }
        public string NickName { get; set; }
        public int OpCode { get; set; }
        public string Province { get; set; }
        public string QQNum { get; set; }
        public int Scene { get; set; }
        public int Sex { get; set; }
        public string Signature { get; set; }
        public string Ticket { get; set; }
        public string UserName { get; set; }
        public int VerifyFlag { get; set; }
    }
}

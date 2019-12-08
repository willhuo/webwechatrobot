using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    /// <summary>
    /// 扫码登陆后返回的跳转地址
    /// </summary>
    public class WaitLoginResponse
    {
        public string RedirectUri { get; set; }
        public string BaseUri { get; set; }
        public string BaseHost { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class SyncCheckResponse
    {
        /// <summary>
        /// 1110：从微信客户端上登出
        /// 1101：从其他设备上登陆了网页微信
        /// 0：正常
        /// </summary>
        public string RetCode { get; set; }

        /// <summary>
        /// 2：有新消息
        /// </summary>
        public string Selector { get; set; }
    }
}

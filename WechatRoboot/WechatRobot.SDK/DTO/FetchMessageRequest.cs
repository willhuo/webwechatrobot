using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class FetchMessageRequest
    {
        public BaseRequest BaseRequest { get; set; }

        public SyncKey SyncKey { get; set; }

        public string rr { get; set; }
    }
}

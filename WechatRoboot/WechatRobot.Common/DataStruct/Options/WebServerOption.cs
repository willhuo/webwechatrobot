using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.Common.DataStruct.Options
{
    public class WebserverOption
    {
        public string DebugListenHost { get; set; }
        public string ReleaseListenHost { get; set; }
        public int ListenPort { get; set; }
    }
}

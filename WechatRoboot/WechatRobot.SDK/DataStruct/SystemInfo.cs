using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WechatRobot.SDK.DataStruct
{
    public class SystemInfo
    {
        /*设备ID*/
        public static string DeviceId = "e" + new Random().NextDouble().ToString("f16").Replace(".", string.Empty).Substring(1);

        /*登录标志位*/
        public static bool IsLogin { get; set; }

        /*微信图片存储位置*/
        public static string WebRootPath { get; set; }
        public static string WechatImagePath
        {
            get
            {
                return WebRootPath + Path.DirectorySeparatorChar + "WechatImages" + Path.DirectorySeparatorChar;
            }
        }
    }
}

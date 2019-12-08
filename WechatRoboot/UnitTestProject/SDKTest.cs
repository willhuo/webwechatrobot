using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WechatRobot.SDK.DTO;

namespace UnitTestProject
{
    [TestClass]
    public class SDKTest
    {
        [TestMethod]
        public void WeChatInitResponseTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var respage = File.ReadAllText("1.txt",Encoding.GetEncoding("GB2312"));

            var weChatInitResponse = JsonConvert.DeserializeObject<WeChatInitResponse>(respage);

            Console.WriteLine("反序列化成功");
        }
    }
}

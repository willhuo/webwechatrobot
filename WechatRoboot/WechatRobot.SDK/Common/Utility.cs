using Dijing.Common.Core.Utility;
using System;
using System.IO;
using System.Xml.Serialization;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Common
{
    public static class Utility
    {
        public static T XmlDeserialize<T>(string inputStr, Action<T> transform = null) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(inputStr))
            {
                var model = (T)ser.Deserialize(sr);
                transform?.Invoke(model);
                return model;
            }
        }

        public static BaseRequest MapperToBaseRequest(LoginResponse loginResponse,string deviceId)
        {
            return new BaseRequest()
            {
                Uin = loginResponse.WxUin.ToString(),
                Sid = loginResponse.WxSid,
                Skey = loginResponse.Skey,
                DeviceID = deviceId
            };
        }

        public static string GetClientMsgId()
        {
            return DateTimeHelper.Default.GetTimestamp(13) + DateTime.Now.Ticks.ToString().Substring(14);
        }
    }
}

using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public interface IWeChatMessageClinet
    {
        /*public method*/
        void BeginSyncMsg();
        void SyncCheck(IWeChatLoginClient weChatLoginClient);
        IResult<SendMessageResponse> SendMessage(LoginResponse loginResponse, string fromUsername, string message, string toUsername);
    }
}

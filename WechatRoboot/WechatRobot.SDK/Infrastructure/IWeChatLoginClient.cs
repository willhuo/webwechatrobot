using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public interface IWeChatLoginClient
    {
        /*attr*/
        LoginResponse LoginResponse { get; }
        SyncKey SyncKey { get; set; }
        string Username { get; }
        User User { get; }
        string[] ChatSetArray { get; }


        /*public method*/
        IResult<string> Login();
        IResult<User> WaitForLogin();
    }
}

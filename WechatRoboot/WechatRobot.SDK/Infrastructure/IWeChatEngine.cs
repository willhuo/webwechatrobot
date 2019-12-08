using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public interface IWeChatEngine
    {
        /*attr*/
        User User { get; }
        List<ContactUser> ContactList { get; }
        List<ContactUser> BatchContactList { get; }


        /*public method*/
        IResult<string> Login();
        IResult<User> WaitForLogin();
        IResult<SendMessageResponse> SendMessage(string message, string toUsername);
    }
}

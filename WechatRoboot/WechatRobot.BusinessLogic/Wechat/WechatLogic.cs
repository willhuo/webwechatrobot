using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WechatRobot.Model.Search;
using WechatRobot.SDK.DTO;
using WechatRobot.SDK.Infrastructure;

namespace WechatRobot.BusinessLogic.Wechat
{
    public class WechatLogic : BaseClass, IWechatLogic
    {
        public WechatLogic(IWeChatEngine weChatEngine)
        {
            this._WeChatEngine = weChatEngine;
        }
        private IWeChatEngine _WeChatEngine { get; set; }


        public IResult<List<ContactUser>> GetContactList(GetContactsSearch search)
        {
            var result = new Result<List<ContactUser>>();
            try
            {
                var list = _WeChatEngine.ContactList.Skip(search.Offset).Take(search.Limit).ToList();
                result.Total = _WeChatEngine.ContactList.Count;
                result.Page = (int)Math.Ceiling(1.0 * result.Total / search.Limit);
                result.SetSuccess();
                result.SetData(list);
                return result;
            }
            catch (Exception ex)
            {
                return base.SetException<List<ContactUser>>(ex);
            }
        }
    }
}

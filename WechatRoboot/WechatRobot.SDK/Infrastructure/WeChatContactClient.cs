using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using WechatRobot.SDK.DataStruct;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public class WeChatContactClient : IWeChatContactClient
    {
        /*constructor*/
        public WeChatContactClient(IWeChatHttpClient weChatHttpClient)
        {
            _WeChatHttpClient = weChatHttpClient;
        }


        /*variable*/
        private IWeChatHttpClient _WeChatHttpClient;
        private ContactResponse _ContactResponse;
        private BatchContactResponse _BatchContactResponse;

         
        /*attr*/
        public List<ContactUser> ContactList
        {
            get
            {
                return _ContactResponse?.MemberList;
            }
        }
        public List<ContactUser> BatchContactList
        {
            get
            {
                return _BatchContactResponse?.ContactList;
            }
        }
        public List<ContactUser> AllMemberList
        {
            get
            {
                if(_ContactResponse==null&& _BatchContactResponse==null)
                {
                    return null;
                }
                else if(_ContactResponse!=null && _BatchContactResponse == null)
                {
                    return _ContactResponse.MemberList;
                }
                else if(_ContactResponse == null && _BatchContactResponse != null)
                {
                    return _BatchContactResponse.ContactList;
                }
                else
                {
                    return _ContactResponse.MemberList.Union(_BatchContactResponse.ContactList).ToList();
                }
            }
        }        


        /*public method*/
        public IResult<ContactResponse> GetContactList(LoginResponse loginResponse)
        {
            //联系人列表获取
            var resultContactList = _WeChatHttpClient.GetContactList(loginResponse);
            if (!resultContactList.Success)
            {
                return resultContactList;
            }
            else
            {
                _ContactResponse = resultContactList.GetData();
            }

            //联系人头像获取
            if (resultContactList.Data.MemberCount > 0)
            {
                foreach (var member in resultContactList.Data.MemberList)
                {
                    var resultHeadPhoto = _WeChatHttpClient.GetHeadPhoto(member.HeadImgUrl);
                    if (resultHeadPhoto.Success)
                    {
                        member.HeadImgBase64 = resultHeadPhoto.GetData();
                    }
                }
            }
            LogHelper.Default.LogDay("联系人头像获取完成");
            LogHelper.Default.LogPrint("联系人头像获取完成", 2);

            return resultContactList;
        }
        public IResult<BatchContactResponse> GetBatchContacts(LoginResponse loginResponse, string[] userNameArray)
        {
            //微信群列表标志校验
            if (userNameArray == null || userNameArray.Length == 0)
            {
                LogHelper.Default.LogDay("微信群UserNameArray为空");
                LogHelper.Default.LogPrint("微信群UserNameArray为空", 3);
                return new Result<BatchContactResponse>()
                {
                    Success = false
                };
            }

            //微信群列表获取
            var result = _WeChatHttpClient.GetBatchContactList(loginResponse, userNameArray);
            if (result.Success)
            {
                if(_BatchContactResponse==null)
                {
                    _BatchContactResponse = result.GetData();
                }
                else
                {
                    _BatchContactResponse.ContactList = _BatchContactResponse.ContactList.Union(result.GetData().ContactList).ToList();
                }
                
                LogHelper.Default.LogPrint($"微信群获取成功，获取群个数为：{_BatchContactResponse.Count}");
            }

            //微信群头像获取
            if (result.Data.Count > 0)
            {
                foreach (var member in result.Data.ContactList)
                {
                    var resultHeadPhoto = _WeChatHttpClient.GetHeadPhoto(member.HeadImgUrl);
                    if (resultHeadPhoto.Success)
                    {
                        member.HeadImgBase64 = resultHeadPhoto.GetData();
                    }
                }
            }
            LogHelper.Default.LogDay("微信群头像获取完成");
            LogHelper.Default.LogPrint("微信群头像获取完成", 2);

            return result;
        }
    }
}

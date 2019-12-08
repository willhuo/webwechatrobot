using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DataStruct
{
    public class UrlEndPoints
    {
        public static string Domain = "https://wx.qq.com";
        public static string GetUUIDUrl = "https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_={0}";   //原始的跳转地址被删除
        public static string GetQRCodeUrl = "https://login.weixin.qq.com/l/{0}";
        public static string WaitLoginUrl = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?tip={0}&uuid={1}&_={2}";
        public static string WeChatInitUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?{0}&lang=en_US&pass_ticket={1}";

        //联系人获取
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket=SfwrioPe8%252BEyaRINXNFpIFQDuGDjJXKvY9aWds98Vz%252Bvuv3DPcxqtdEbBl5%252BfBmL&r=1554525287734&seq=0&skey=@crypt_707cf3d1_9156d632108c64d3d38dfb05e3e67806
        public static string GetContactsUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?pass_ticket={0}&skey={1}&r={2}";

        //微信群获取
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r=1554525288593&pass_ticket=SfwrioPe8%252BEyaRINXNFpIFQDuGDjJXKvY9aWds98Vz%252Bvuv3DPcxqtdEbBl5%252BfBmL
        public static string GetBatchContactsUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxbatchgetcontact?type=ex&r={0}&pass_ticket={1}";

        public static string SyncCheckUrl = "https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?sid={0}&uin={1}&synckey={2}&r={3}&skey={4}&deviceid={5}&_={6}";

        //消息同步地址
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid=e55IwnZUVzY69hb5&skey=@crypt_707cf3d1_a04293944e58b70a669b0addcd90be61&pass_ticket=MAJT6Iq4myptVj6aDkSW4NNFdThQRH9XVGrVhfsj2rFhK6gwmz8o%252B1kw%252FF0AY4Pa
        public static string FetchMessageUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={0}&skey={1}&pass_ticket={2}";
        
        //获取图片地址，正常图片是big，小视频的图片是slave
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=7906731013334286043&skey=%40crypt_707cf3d1_a04293944e58b70a669b0addcd90be61&type=slave
        public static string FetchImageMessageUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID={0}&skey={1}&type={2}";

        //获取短视频
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvideo?msgid=5814070609426911830&skey=%40crypt_707cf3d1_5a5c3d32af76173adcd73643083dbe65
        public static string FetchVideoMessageUrl = " https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvideo?msgid={0}&skey={1}";

        //获取语音消息
        //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvoice?msgid=3939646516786056491&skey=@crypt_707cf3d1_5a5c3d32af76173adcd73643083dbe65
        public static string FetchAudioMessageUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetvoice?msgid={0}&skey={1}";

        //文件下载
        //https://file.wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmedia?sender=@8f41a5e37e6f1e1b5c0e356874e01dd4&mediaid=@crypt_50bf2972_acf3f16fefa8d9f0552b4f377be4c179bcf31d54aa74c7e9745550b35c29b86fffe8d4e1f245b749dfdb3bfde99d5285f28584bf78ba1d52ea236e50afe70b3ec327d4caa703bfd8cf98848eaaac672a09083cccd96a098706d27b77492caf8128eee3439fef89e26b1528315c955dacd6b4b9272cadd5b683604cd679efd53bc67a71c294e3246624a6ccfe74da29fbac7cb304ae555ed701b961949969ea970ef6406aa5159a688f06cdf69944d5ccf9a5028fe91ab84a01ea4a4429b1272504e6a95d19ce45adba2d1d0318456e90&encryfilename=%E5%B8%82%E5%A7%94%E5%8A%9E%E5%85%AC%E5%AE%A4%E3%80%81%E5%B8%82%E6%94%BF%E5%BA%9C%E5%8A%9E%E5%85%AC%E5%AE%A4%E5%8D%B0%E5%8F%91%E3%80%8A%E5%85%B3%E4%BA%8E%E5%8A%A0%E5%BF%AB%E6%8E%A8%E8%BF%9B%E5%9F%8E%E4%B9%A1%E5%BB%BA%E8%AE%BE%E7%94%A8%E5%9C%B0%E5%A2%9E%E5%87%8F%E6%8C%82%E9%92%A9%E7%AD%89%E7%9B%B8%E5%85%B3%E5%B7%A5%E4%BD%9C%E7%9A%84%E5%AE%9E%E6%96%BD%E6%84%8F%E8%A7%81%E3%80%8B%E7%9A%84%E9%80%9A%E7%9F%A5%2Epdf&pass_ticket=A3e2Yi23TkykNMX27ZvH4Jxg1qlHIfsLX%252BEE8HLPXFQGJcaLCezvr6CsF9iL5HoS
        public static string FetchFileMessageUrl = "https://file.wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetmedia?sender={0}&mediaid={1}&encryfilename={2}&pass_ticket={3}";

        public static string SendMessageUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?pass_ticket={0}";
    }
}

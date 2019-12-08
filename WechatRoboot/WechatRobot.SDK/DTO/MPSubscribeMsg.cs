using System;
using System.Collections.Generic;
using System.Text;

namespace WechatRobot.SDK.DTO
{
    public class MPSubscribeMsg
    {
        public int MPArticleCount { get; set; }
        public List<MPArticle> MPArticleList { get; set; }
        public string NickName { get; set; }
        public long Time { get; set; }
        public string UserName { get; set; }
    }

    public class MPArticle
    {
        public string Cover { get; set; }
        public string Digest { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}

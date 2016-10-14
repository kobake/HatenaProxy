using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HatenaProxy.Controllers.api
{
    public class TimelineItem
    {
        public string ArticleName { get; set; }
        public string ArticleUrl { get; set; }

        public string HatenaUrl { get; set; }
        public int BookmarkCount { get; set; }

        public string UserIcon { get; set; }
        public string UserId { get; set; }
        public string UserComment { get; set; }
        public int UserStar { get; set; }
    }
    public class TimelineResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }
        public string MyUserId { get; set; }
        public List<TimelineItem> Timeline { get; set; }
    }

    public class TimelineController : ApiController
    {
        // api/timeline?user=kobake
        public object Get()
        {
            // http://b.hatena.ne.jp/kobake/ … 自分の発言
            // http://b.hatena.ne.jp/kobake/favorite … フォロイーの発言
            try
            {
                // クエリパラメータ (user)
                string user = Request.GetQueryNameValuePairs().Where(p => p.Key == "user").Select(p => p.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(user)) throw new Exception("Required query parameter 'user'");

                var response = new TimelineResponse
                {
                    Result = "OK",
                    Error = "",
                    MyUserId = user,
                    Timeline = new List<TimelineItem>
                    {
                        new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                        new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                        new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                var response = new TimelineResponse
                {
                    Result = "Error",
                    Error = ex.Message,
                    MyUserId = "",
                    Timeline = new List<TimelineItem>()
                };
                return response;
            }

        }
    }
}

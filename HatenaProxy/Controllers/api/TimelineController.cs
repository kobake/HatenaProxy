using CsQuery;
using HatenaProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace HatenaProxy.Controllers.api
{
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
        public async Task<object> Get()
        {
            // http://b.hatena.ne.jp/kobake/ … 自分の発言
            // http://b.hatena.ne.jp/kobake/favorite … フォロイーの発言
            try
            {
                // クエリパラメータ (user)
                string user = Request.GetQueryNameValuePairs().Where(p => p.Key == "user").Select(p => p.Value).FirstOrDefault();
                if (string.IsNullOrEmpty(user)) throw new Exception("Required query parameter 'user'");

                // 値検証
                if(!Regex.IsMatch(user, @"^[A-Za-z0-9_\-]+$")) throw new Exception("Invalid query parameter 'user'");

                // タイムライン生成
                List<TimelineItem> timeline = await TimelineParser.Generate(user);
                
                /*
                new List<TimelineItem>
                {
                    new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                    new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                    new TimelineItem {ArticleName="",ArticleUrl="",BookmarkCount=0,UserIcon="http", UserId="kobake",UserComment="Hello", UserStar=0 },
                }
                */

                var response = new TimelineResponse
                {
                    Result = "OK",
                    Error = "",
                    MyUserId = user,
                    Timeline = timeline
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

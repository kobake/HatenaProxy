using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HatenaProxy.Models
{
    public class HatenaComment
    {
        public string UserId { get; set; }
        public string Comment { get; set; }

        public string UserImage
        {
            get
            {
                return $"http://cdn1.www.st-hatena.com/users/{UserId.Substring(0, 2)}/{UserId}/profile.gif";
            }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set
            {
                date = value.Trim();
                try
                {
                    string[] tmp = date.Split('/');
                    date = string.Format("{0:D4}/{1:D2}/{2:D2}", int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2])); // YYYY/MM/DD に揃える
                }
                catch (Exception)
                {
                    // 例外が生じた場合はそのまま使う
                }
            }
        }
    }
    public class TimelineItem
    {
        public string ArticleName { get; set; }
        public string ArticleUrl { get; set; }

        public string HatenaUrl
        {
            get
            {
                // "http://b.hatena.ne.jp/entry/negineesan.hatenablog.com/entry/2016/10/12/202541"
                string url = ArticleUrl;
                url = Regex.Replace(url, "https?://", "");
                url = "http://b.hatena.ne.jp/entry/" + url;
                return url;
            }
        }
        public int BookmarkCount { get; set; }

        public HatenaComment Comment { get; set; }
    }

    public class TimelineParser
    {

        public static async Task<List<TimelineItem>> Generate(string user)
        {
            string otherTimelineUrl = $"http://b.hatena.ne.jp/{user}/favorite";
            string myTimelineUrl = $"http://b.hatena.ne.jp/{user}/";
            List<TimelineItem> otherTimelines = await _GetTimelineListFromUrl(otherTimelineUrl);
            List<TimelineItem> myTimelines = await _GetTimelineListFromUrl(myTimelineUrl);

            // 結合
            List<TimelineItem> ret = new List<TimelineItem>();
            ret.AddRange(otherTimelines);
            ret.AddRange(myTimelines);

            // ソート (日付降順)
            ret = ret.OrderByDescending(item => item.Comment.Date).ToList();
            return ret;
        }

        private static async Task<List<TimelineItem>> _GetTimelineListFromUrl(string url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Encoding = Encoding.GetEncoding("UTF-8");
            string html = await client.DownloadStringTaskAsync(url);

            List<TimelineItem> ret = new List<TimelineItem>();
            var cq = new CQ(html);
            cq[".entrylist-unit"].Each((_entry) => {
                var item = _TimelineItem(_entry);
                ret.AddRange(item);
            });
            return ret;
        }

        private static List<TimelineItem> _TimelineItem(IDomObject _entry)
        {
            List<TimelineItem> ret = new List<TimelineItem>();

            // コメント情報
            var entry = new CQ(_entry.InnerHTML);
            entry["ul.comment>li"].Each((_comment) =>
            {
                var item = new TimelineItem();

                // 記事情報
                var link = entry[".entry-link"];
                item.ArticleName = link.Text();
                item.ArticleUrl = link.Attr("href");
                int bookmarkCount = 0;
                int.TryParse(_entry.Attributes["data-bookmark-count"], out bookmarkCount);
                item.BookmarkCount = bookmarkCount;

                // コメント情報
                item.Comment = _HatenaComment(_comment);
                if (!string.IsNullOrEmpty(item.Comment.Comment)) // コメント文があるものだけをリストに追加
                {
                    ret.Add(item);
                }
            });
            return ret;
        }
        private static HatenaComment _HatenaComment(IDomObject _comment)
        {
            HatenaComment ret = new HatenaComment();
            var comment = new CQ(_comment.OuterHTML);
            ret.UserId = _comment.Attributes["data-user"];
            ret.Date = comment["span.timestamp"].Text().Trim();
            ret.Comment = comment["span.comment"].Text().Trim();
            return ret;
        }
    }
}
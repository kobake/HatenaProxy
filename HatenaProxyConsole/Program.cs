using CsQuery;
using HatenaProxy.Controllers.api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HatenaProxyConsole
{
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
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("UTF-8");

            Task.Run(async () => {

                // 情報取得
                string user = "kobake";

                
                try{
                    if (true)
                    {
                        List<TimelineItem> items = await TimelineParser.Generate(user);
                        string json = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
                        Console.WriteLine(json);
                        return;
                    }
                    if(false){
                        string myTimelineUrl = $"http://b.hatena.ne.jp/{user}/";
                        Console.WriteLine(myTimelineUrl);

                        WebClient client = new WebClient();
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        client.Encoding = Encoding.GetEncoding("UTF-8");
                        string html = await client.DownloadStringTaskAsync(myTimelineUrl);

                        var cq = new CQ(html);
                        cq[".entrylist-unit"].Each((_entry) => {
                            Console.WriteLine("--------------------");
                            var entry = new CQ(_entry.InnerHTML);
                            var link = entry[".entry-link"];
                            Console.WriteLine(link.Text());

                            entry["ul.comment>li"].Each((_comment) => {
                                var comment = new CQ(_comment.OuterHTML);
                                var commentUser = _comment.Attributes["data-user"];
                                Console.WriteLine(commentUser);
                                var image = $"http://cdn1.www.st-hatena.com/users/{commentUser.Substring(0, 2)}/{commentUser}/profile.gif";
                                Console.WriteLine(image);
                                var timestamp = comment["span.timestamp"].Text().Trim();
                                Console.WriteLine(timestamp);
                                var commentBody = comment["span.comment"].Text().Trim();
                                Console.WriteLine(commentBody);
                                Console.WriteLine("");
                            });
                            Console.WriteLine("");
                        });

                        return;
                    }

                    if (false)
                    {
                        string otherTimelineUrl = $"http://b.hatena.ne.jp/{user}/favorite";
                        Console.WriteLine(otherTimelineUrl);

                        WebClient client = new WebClient();
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        client.Encoding = Encoding.GetEncoding("UTF-8");
                        string html = await client.DownloadStringTaskAsync(otherTimelineUrl);

                        var cq = new CQ(html);
                        cq[".entrylist-unit"].Each((_entry) => {
                            Console.WriteLine("--------------------");
                            var entry = new CQ(_entry.InnerHTML);
                            var link = entry[".entry-link"];
                            Console.WriteLine(link.Text());

                            entry["ul.comment>li"].Each((_comment) => {
                                var comment = new CQ(_comment.OuterHTML);
                                var commentUser = _comment.Attributes["data-user"];
                                Console.WriteLine(commentUser);
                                var image = $"http://cdn1.www.st-hatena.com/users/{commentUser.Substring(0, 2)}/{commentUser}/profile.gif";
                                Console.WriteLine(image);
                                var timestamp = comment["span.timestamp"].Text().Trim();
                                Console.WriteLine(timestamp);
                                var commentBody = comment["span.comment"].Text().Trim();
                                Console.WriteLine(commentBody);
                                Console.WriteLine("");
                            });
                            Console.WriteLine("");
                        });

                        return;
                    }

                    if (false){
                        string myTimelineUrl = "";
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                        //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "UTF-8");
                        var response = await client.GetAsync(myTimelineUrl);

                        string html = "";
                        using (var stream = (await response.Content.ReadAsStreamAsync()))
                        using (var reader = (new StreamReader(stream, Encoding.GetEncoding("UTF-8"), true)) as TextReader)
                        {
                            html = await reader.ReadToEndAsync();
                        }

                        Console.WriteLine(html.Substring(0, 400));
                        return;
                        var cq = new CQ(html);
                        cq[".entrylist-unit"].Each((e) => {
                            Console.WriteLine("--------------------");
                            Console.WriteLine(e.InnerText);
                            Console.WriteLine("");
                        });
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }).Wait();
        }
    }
}

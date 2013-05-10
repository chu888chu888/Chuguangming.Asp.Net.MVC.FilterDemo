using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using CnBlogsCsdnArticles.Models;

namespace CnBlogsCsdnArticles.Controllers
{
    public class IndexController : Controller
    {
        /// <summary>
        /// 文章集合
        /// </summary>
        private List<BlogItemModel> listModel = null;
        WebClient wc = new WebClient();
        //读取地址
        string address = string.Empty;

        public ActionResult Index()
        {
            getPageIndex("1");
            return View();
        }
     
        [HttpPost]
        public ActionResult Index(BlogModel model, FormCollection collection)
        {
            getPageIndex(collection["PageIndex"]);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            listModel = new List<BlogItemModel>();

            BlogSearchModel search = model.Search;
            search.PageIndex = Convert.ToInt32(collection["PageIndex"]);
            //MusicStore
            search.Title = string.IsNullOrWhiteSpace(search.Title) ? string.Empty : search.Title.Trim();
            search.Content = string.IsNullOrWhiteSpace(search.Content) ? string.Empty : search.Content.Trim();
            search.RecommendGreaterThan = search.RecommendGreaterThan == null ? "0" : search.RecommendGreaterThan.Trim();
            search.CommentsGreaterThan = search.CommentsGreaterThan == null ? "0" : search.CommentsGreaterThan;
            search.ReadsGreaterThan = search.ReadsGreaterThan == null ? "0" : search.ReadsGreaterThan;

            for (int i = 1; i <= search.PageIndex; i++) //翻页数量
            {
                if (search.IsCnBlogsSelect)
                {
                    CnBlog(search, i);
                }

                if (search.IsCsdnSelect)
                {
                    Csdn(search, i);
                }
            }

            model.Items = listModel;

            ViewBag.TotalCount = listModel.Count;
            return View(model);
        }

        /// <summary>
        /// 读取博客园数据
        /// </summary>
        private void CnBlog(BlogSearchModel search, int i)
        {
            address = "http://www.cnblogs.com/p" + i.ToString();

            //实例化HtmlAgilityPack.HtmlDocument对象
            HtmlDocument doc = getHtmlDoc(address);

            //根据HTML节点NODE的ID获取节点
            HtmlNode navNode = doc.GetElementbyId("post_list");
            //div[2]表示文章链接a位于post_list里面第3个div节点中
            //HtmlNodeCollection list = navNode.SelectNodes("//div[2]/h3/a");  //根据XPATH来索引节点

            HtmlNodeCollection list = navNode.SelectNodes("div");  //根据XPATH来索引节点
            foreach (HtmlNode item in list)
            {
                var model = new BlogItemModel
                {
                    RecommendedCounts = getDigitInString(item.SelectSingleNode("div[1]/div/span").InnerText),
                    Title = item.SelectSingleNode("div[2]/h3/a").InnerText,
                    TitleUrl = item.SelectSingleNode("div[2]/h3/a").Attributes["href"].Value,
                    Content = item.SelectSingleNode("div[2]/p").InnerText,
                    Avatar = item.SelectSingleNode("div[2]/p/a/img") == null ? null : item.SelectSingleNode("div[2]/p/a/img").Attributes["src"].Value,
                    Author = item.SelectSingleNode("div[2]/div/a").InnerText,
                    AuthorUrl = item.SelectSingleNode("div[2]/div/a").Attributes["href"].Value,
                    PublishDate = item.SelectSingleNode("div[2]/div").InnerText,
                    Comments = getDigitInString(item.SelectSingleNode("div[2]/div/span[1]/a").InnerText),
                    Reads = getDigitInString(item.SelectSingleNode("div[2]/div/span[2]/a").InnerText),
                    WebSiteSource = "博客园"
                };


                if (model.RecommendedCounts >= Convert.ToInt32(search.RecommendGreaterThan) &&
                    model.Comments >= Convert.ToInt32(search.CommentsGreaterThan) &&
                    model.Reads >= Convert.ToInt32(search.ReadsGreaterThan))
                {
                    if (search.Title != string.Empty)
                    {
                        if (model.Title.Contains(search.Title))
                        {
                            listModel.Add(model);
                        }
                    }
                    else if (search.Content != string.Empty)
                    {
                        if (model.Content.Contains(search.Content))
                        {
                            listModel.Add(model);
                        }
                    }
                    else
                    {
                        listModel.Add(model);
                    }
                }
            }
        }

        /// <summary>
        /// 读取Csdn数据
        /// </summary>
        private void Csdn(BlogSearchModel search, int i)
        {
            address = "http://blog.csdn.net/hot.html?page=" + i.ToString();

            //实例化HtmlAgilityPack.HtmlDocument对象
            HtmlDocument doc = getHtmlDoc(address);

            //根据HTML节点NODE的ID获取节点
            HtmlNode navNode = doc.GetElementbyId("wrap");
            //div[2]表示文章链接a位于post_list里面第3个div节点中
            //HtmlNodeCollection list = navNode.SelectNodes("//div[2]/h3/a");  //根据XPATH来索引节点

            HtmlNodeCollection list = navNode.SelectNodes("div[3]/div[2]/div");  //根据XPATH来索引节点
            foreach (HtmlNode item in list)
            {
                if (item.SelectSingleNode("h1/a") == null) //碰到翻页信息时跳出
                {
                    break;
                }
                BlogItemModel model = new BlogItemModel();
                model.ArticleType = item.SelectSingleNode("h1/a[1]") == null ? string.Empty : item.SelectSingleNode("h1/a[1]").InnerText;
                model.RecommendedCounts = null;  //Csdn博客无推荐功能
                model.Title = item.SelectSingleNode("h1/a[2]") == null ? item.SelectSingleNode("h1/a").InnerText : item.SelectSingleNode("h1/a[2]").InnerText;
                model.TitleUrl = item.SelectSingleNode("h1/a[2]") == null ? item.SelectSingleNode("h1/a").Attributes["href"].Value : item.SelectSingleNode("h1/a[2]").Attributes["href"].Value;
                model.Content = item.SelectSingleNode("dl/dd") == null ? string.Empty : item.SelectSingleNode("dl/dd").InnerText;
                model.Avatar = item.SelectSingleNode("dl/dt/a/img") == null ? null : item.SelectSingleNode("dl/dt/a/img").Attributes["src"].Value;
                model.Author = item.SelectSingleNode("div/span/a[1]") == null ? string.Empty : item.SelectSingleNode("div/span/a[1]").InnerText;
                model.AuthorUrl = item.SelectSingleNode("div/span/a[1]") == null ? string.Empty : item.SelectSingleNode("div/span/a[1]").Attributes["href"].Value;
                model.PublishDate = item.SelectSingleNode("div/span/span") == null ? string.Empty : item.SelectSingleNode("div/span/span").InnerText;
                model.Comments = item.SelectSingleNode("div/span/a[3]") == null ? null : getDigitInString(item.SelectSingleNode("div/span/a[3]").InnerText);
                model.Reads = item.SelectSingleNode("div/span/a[2]") == null ? null : getDigitInString(item.SelectSingleNode("div/span/a[2]").InnerText);
                model.WebSiteSource = "CSDN";

                if (model.Comments >= Convert.ToInt32(search.CommentsGreaterThan) &&
                    model.Reads >= Convert.ToInt32(search.ReadsGreaterThan))
                {
                    if (search.Title != string.Empty)
                    {
                        if (model.Title.Contains(search.Title))
                        {
                            listModel.Add(model);
                        }
                    }
                    else if (search.Content != string.Empty)
                    {
                        if (model.Content.Contains(search.Content))
                        {
                            listModel.Add(model);
                        }
                    }
                    else
                    {
                        listModel.Add(model);
                    }
                }
            }
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startChar"></param>
        /// <param name="endChar"></param>
        /// <returns></returns>
        private int? getDigitInString(string str, char startChar = '(', char endChar = ')')
        {
            int start = str.IndexOf('(');
            int end = str.IndexOf(')');
            if (start == -1 || end == -1)
            {
                return Convert.ToInt32(str);
            }
            return Convert.ToInt32(str.Substring(start + 1, end - start - 1));
        }

        /// <summary>
        /// 从地址下载html，并返回htmlDom对象
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private HtmlDocument getHtmlDoc(string address)
        {
            Stream stream = wc.OpenRead(address);
            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            string html = sr.ReadToEnd();
            sr.Close();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private void getPageIndex(string selectedValue)
        {
            List<DdlItem> items = new List<DdlItem>();
            for (int i = 1; i <= 10; i++)
            {
                items.Add(
                      new DdlItem
                      {
                          Key = i.ToString(),
                          Value = i.ToString()
                      }
                    );
            }

            ViewBag.PageIndex = new SelectList(items, "Key", "Value", selectedValue);
        }
    }

    /// <summary>
    /// 用于显示下拉框
    /// </summary>
    public class DdlItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

}

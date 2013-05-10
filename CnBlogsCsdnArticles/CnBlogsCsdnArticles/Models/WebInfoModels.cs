using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace CnBlogsCsdnArticles.Models
{

    public class BlogModel
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public BlogSearchModel Search { get; set; }

        /// <summary>
        /// 列表项
        /// </summary>
        public List<BlogItemModel> Items { get; set; }
    }

    /// <summary>
    /// 查询条件
    /// </summary>
    public class BlogSearchModel
    {
        /// <summary>
        /// 是否显示博客园
        /// </summary>
        [Display(Name = "是否显示博客园")]
        public bool IsCnBlogsSelect { get; set; }

        /// <summary>
        /// 是否显示CSDN
        /// </summary>
        [Display(Name = "是否显示CSDN")]
        public bool IsCsdnSelect { get; set; }

        /// <summary>
        /// 推荐数大于等于
        /// </summary>
        [Display(Name = "推荐数大于等于")]
        [Range(0, int.MaxValue, ErrorMessage = "请输入正整数")]
        public string RecommendGreaterThan { get; set; }

        /// <summary>
        /// 评论数大于等于
        /// </summary>
        [Display(Name = "评论数大于等于")]
        [Range(0, int.MaxValue, ErrorMessage = "请输入正整数")]
        public string CommentsGreaterThan { get; set; }

        /// <summary>
        /// 阅读数大于等于
        /// </summary>
        [Display(Name = "阅读数大于等于")]
        [Range(0, int.MaxValue, ErrorMessage = "请输入正整数")]
        public string ReadsGreaterThan { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 翻页数量
        /// </summary>
        [Display(Name = "翻页数量")]
        public int? PageIndex { get; set; }
    }

    /// <summary>
    /// 单条博客内容
    /// </summary>
    public class BlogItemModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 标题链接
        /// </summary>
        public string TitleUrl { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 作者链接
        /// </summary>
        public string AuthorUrl { get; set; }

        /// <summary>
        /// 发布时间 
        /// </summary>
        public string PublishDate { get; set; }

        /// <summary>
        /// 被推荐数
        /// </summary>
        public int? RecommendedCounts { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int? Comments { get; set; }

        /// <summary>
        /// 阅读数
        /// </summary>
        public int? Reads { get; set; }

        /// <summary>
        /// 所属类型
        /// </summary>
        public string ArticleType { get; set; }

        /// <summary>
        /// 网站来源
        /// </summary>
        public string WebSiteSource { get; set; }
    }

}

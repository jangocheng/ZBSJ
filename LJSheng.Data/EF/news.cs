namespace LJSheng.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 新闻资讯
    /// </summary>
    [Table("news")]
    public partial class news
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid gid { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime add_time { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// 是否显示[1=显示 2=不显示 3=首页显示]
        /// </summary>
        public int show { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [StringLength(50)]
        public string picture { get; set; }

        /// <summary>
        /// 图文详情
        /// </summary>
        [StringLength(50)]
        public string graphic_details { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        [StringLength(200)]
        public string subtitle { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(50)]
        public string title { get; set; }

        /// <summary>
        /// 外链地址
        /// </summary>
        [StringLength(200)]
        public string url { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 作者/来源
        /// </summary>
        [StringLength(50)]
        public string author { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int number { get; set; }
    }
}

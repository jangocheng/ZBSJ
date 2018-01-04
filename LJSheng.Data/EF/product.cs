using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 产品列表
    /// </summary>
    public partial class product
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
        /// 名称
        /// </summary>
        [StringLength(200)]
        public string name { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal original_price { get; set; }

        /// <summary>
        /// 副标题/简介
        /// </summary>
        [StringLength(800)]
        public string subtitle { get; set; }

        /// <summary>
        /// 详情介绍
        /// </summary>
        public string content { get; set; }

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
        /// 分类外键
        /// </summary>
        public Guid? classify_gid { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string remarks { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 扩展[课程/视频老师]
        /// </summary>
        [StringLength(50)]
        public string extend1 { get; set; }

        /// <summary>
        /// 扩展[课程开始时间/视频时长]
        /// </summary>
        [StringLength(50)]
        public string extend2 { get; set; }

        /// <summary>
        /// 扩展[课程培训地址/视频播放地址]
        /// </summary>
        [StringLength(500)]
        public string extend3 { get; set; }

        /// <summary>
        /// 扩展[课程人数/视频可观看次数/真柏库存]
        /// </summary>
        public int? extend4 { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 评论
    /// </summary>
    public partial class comment
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
        /// 是否显示
        /// </summary>
        public int show { get; set; }

        /// <summary>
        /// 会员GID
        /// </summary>
        public Guid member_gid { get; set; }

        /// <summary>
        /// 产品GID
        /// </summary>
        public Guid product_gid { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [StringLength(800)]
        public string content { get; set; }

        /// <summary>
        /// 回复
        /// </summary>
        [StringLength(800)]
        public string reply { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? reply_time { get; set; }

        /// <summary>
        /// 图片列表
        /// </summary>
        [StringLength(200)]
        public string picture { get; set; }
    }
}

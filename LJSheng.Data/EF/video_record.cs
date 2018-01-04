using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 视频观看记录
    /// </summary>
    public partial class video_record
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
        /// 会员GID
        /// </summary>
        public Guid member_gid { get; set; }

        /// <summary>
        /// 产品GID
        /// </summary>
        public Guid product_gid { get; set; }

        /// <summary>
        /// 剩余次数
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 最近观看时间
        /// </summary>
        public DateTime video_time { get; set; }
    }
}

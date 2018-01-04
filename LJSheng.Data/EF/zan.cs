using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 点赞表
    /// </summary>
    public partial class zan
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
    }
}

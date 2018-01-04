using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 分类
    /// </summary>
    public partial class classify
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
        /// 类型[1=真柏 2=视频 3=课程]
        /// </summary>
        public int ltype { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string remarks { get; set; }
    }
}

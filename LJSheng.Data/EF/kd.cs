using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 视频购买记录
    /// </summary>
    public partial class kd
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
        /// 是否显示[1=显示 2=不显示]
        /// </summary>
        public int show { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }

        /// <summary>
        /// 查询编码
        /// </summary>
        [StringLength(50)]
        public string type { get; set; }

        /// <summary>
        /// 首字母
        /// </summary>
        [StringLength(50)]
        public string letter { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [StringLength(50)]
        public string tel { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [StringLength(50)]
        public string number { get; set; }
    }
}

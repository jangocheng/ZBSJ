using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 管理员
    /// </summary>
    public partial class ljsheng
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
        /// 帐号
        /// </summary>
        [Required, MaxLength(50)]
        public string account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(50)]
        public string pwd { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [StringLength(20)]
        public string real_name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(50)]
        public string contact_number { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(2)]
        public string gender { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [StringLength(50)]
        public string picture { get; set; }

        /// <summary>
        /// 权限[管理员 锁定]
        /// </summary>
        [StringLength(50)]
        public string jurisdiction { get; set; }

        /// <summary>
        /// 最后登录标识
        /// </summary>
        [StringLength(50)]
        public string login_identifier { get; set; }
    }
}

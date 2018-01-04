using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 会员表
    /// </summary>
    public partial class member
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
        /// 微信ID
        /// </summary>
        [StringLength(50)]
        public string openid { get; set; }

        /// <summary>
        /// 推荐人GID
        /// </summary>
        public Guid? member_gid { get; set; }

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
        /// 省份
        /// </summary>
        [StringLength(20)]
        public string province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(20)]
        public string city { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        [StringLength(20)]
        public string area { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string address { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? date_birth { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [StringLength(200)]
        public string picture { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20)]
        public string nickname { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal balance { get; set; }

        /// <summary>
        /// 权限[正常 审核中 锁定]
        /// </summary>
        [StringLength(50)]
        public string jurisdiction { get; set; }

        /// <summary>
        /// 注册IP
        /// </summary>
        [StringLength(50)]
        public string ip { get; set; }

        /// <summary>
        /// 最后登录标识
        /// </summary>
        [StringLength(50)]
        public string login_identifier { get; set; }

        /// <summary>
        /// 剩余观看视频数量
        /// </summary>
        public int number { get; set; }
    }
}

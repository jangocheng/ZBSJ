using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 短信
    /// </summary>
    public partial class sms
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
        /// 手机号码
        /// </summary>
        [StringLength(50)]
        public string phone_number { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [StringLength(500)]
        public string content { get; set; }

        /// <summary>
        /// 回执内容
        /// </summary>
        [StringLength(200)]
        public string receipt_content { get; set; }

        /// <summary>
        /// 短信类型[1=验证码 2=系统短信]
        /// </summary>
        public int ltype { get; set; }

        /// <summary>
        /// 计费条数
        /// </summary>
        public int number { get; set; }
    }
}

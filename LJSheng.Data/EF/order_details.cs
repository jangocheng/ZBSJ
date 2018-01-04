using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 订单详情
    /// </summary>
    public partial class order_details
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
        /// 订单号的GID
        /// </summary>
        public Guid order_gid { get; set; }

        /// <summary>
        /// 产品GID
        /// </summary>
        public Guid product_gid { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public decimal pay_price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string remarks { get; set; }
    }
}

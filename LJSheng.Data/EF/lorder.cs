using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// 订单
    /// </summary>
    public partial class lorder
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
        public Guid? product_gid { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Required]
        [StringLength(50)]
        public string order_no { get; set; }

        /// <summary>
        /// 网银订单号
        /// </summary>
        [StringLength(50)]
        public string trade_no { get; set; }

        /// <summary>
        /// 支付对账时间
        /// </summary>
        public DateTime? pay_time { get; set; }

        /// <summary>
        /// 支付状态[1=支付成功 2=未支付 3=已退款 4=交易关闭 5=支付成功但金额不对]
        /// </summary>
        public int pay_status { get; set; }

        /// <summary>
        /// 支付类型[1=支付宝 2=微信 3=线下汇款]
        /// </summary>
        public int pay_type { get; set; }

        /// <summary>
        /// 订单支付总金额
        /// </summary>
        public decimal total_price { get; set; }

        /// <summary>
        /// 实际支付金额
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 网上支付金额
        /// </summary>
        public decimal pay_price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 优惠券抵扣金额
        /// </summary>
        public decimal coupon_price { get; set; }

        /// <summary>
        /// 使用的优惠券号码
        /// </summary>
        [StringLength(20)]
        public string coupon_no { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string remarks { get; set; }

        /// <summary>
        /// 支付参数
        /// </summary>
        [StringLength(2000)]
        public string pay { get; set; }

        /// <summary>
        /// 快递状态[1=发货中 2=快递中 3=已签收 4=退回]
        /// </summary>
        public int express_status { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        [StringLength(50)]
        public string express { get; set; }

        /// <summary>
        /// 快递号
        /// </summary>
        [StringLength(50)]
        public string express_number { get; set; }

        /// <summary>
        /// 快递地址
        /// </summary>
        [StringLength(100)]
        public string address { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [StringLength(50)]
        public string consignee { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(50)]
        public string contact_number { get; set; }
    }
}
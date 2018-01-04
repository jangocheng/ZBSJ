using System;
using System.ComponentModel.DataAnnotations;

namespace LJSheng.Data
{
    /// <summary>
    /// ����
    /// </summary>
    public partial class lorder
    {
        /// <summary>
        /// ����
        /// </summary>
        [Key]
        public Guid gid { get; set; }

        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime add_time { get; set; }

        /// <summary>
        /// ��ԱGID
        /// </summary>
        public Guid member_gid { get; set; }

        /// <summary>
        /// ��ƷGID
        /// </summary>
        public Guid? product_gid { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [Required]
        [StringLength(50)]
        public string order_no { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        [StringLength(50)]
        public string trade_no { get; set; }

        /// <summary>
        /// ֧������ʱ��
        /// </summary>
        public DateTime? pay_time { get; set; }

        /// <summary>
        /// ֧��״̬[1=֧���ɹ� 2=δ֧�� 3=���˿� 4=���׹ر� 5=֧���ɹ�������]
        /// </summary>
        public int pay_status { get; set; }

        /// <summary>
        /// ֧������[1=֧���� 2=΢�� 3=���»��]
        /// </summary>
        public int pay_type { get; set; }

        /// <summary>
        /// ����֧���ܽ��
        /// </summary>
        public decimal total_price { get; set; }

        /// <summary>
        /// ʵ��֧�����
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// ����֧�����
        /// </summary>
        public decimal pay_price { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// �Ż�ȯ�ֿ۽��
        /// </summary>
        public decimal coupon_price { get; set; }

        /// <summary>
        /// ʹ�õ��Ż�ȯ����
        /// </summary>
        [StringLength(20)]
        public string coupon_no { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        [StringLength(500)]
        public string remarks { get; set; }

        /// <summary>
        /// ֧������
        /// </summary>
        [StringLength(2000)]
        public string pay { get; set; }

        /// <summary>
        /// ���״̬[1=������ 2=����� 3=��ǩ�� 4=�˻�]
        /// </summary>
        public int express_status { get; set; }

        /// <summary>
        /// ��ݹ�˾
        /// </summary>
        [StringLength(50)]
        public string express { get; set; }

        /// <summary>
        /// ��ݺ�
        /// </summary>
        [StringLength(50)]
        public string express_number { get; set; }

        /// <summary>
        /// ��ݵ�ַ
        /// </summary>
        [StringLength(100)]
        public string address { get; set; }

        /// <summary>
        /// �ջ���
        /// </summary>
        [StringLength(50)]
        public string consignee { get; set; }

        /// <summary>
        /// ��ϵ�绰
        /// </summary>
        [StringLength(50)]
        public string contact_number { get; set; }
    }
}
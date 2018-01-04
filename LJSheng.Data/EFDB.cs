using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace LJSheng.Data
{
    public class EFDB : DbContext
    {
        /// <summary>
        /// EF对象
        /// </summary>
        public EFDB(): base("name=MSSQL")
        {
            //模型更改时重新创建数据库
            //Database.SetInitializer<EFDB>(new DropCreateDatabaseIfModelChanges<EFDB>());
            ////数据库不存在时重新创建数据库,存在的话会报错
            //Database.SetInitializer<EFDB>(new CreateDatabaseIfNotExists<EFDB>());
            ////每次启动应用程序时创建数据库
            //Database.SetInitializer<EFDB>(new DropCreateDatabaseAlways<EFDB>());
            ////从不创建数据库
            Database.SetInitializer<EFDB>(null);
        }

        /// <summary>
        /// 禁止创建表的时候表名复数
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<ljsheng> ljsheng { get; set; }
        public DbSet<member> member { get; set; }
        public DbSet<classify> classify { get; set; }
        public DbSet<comment> comment { get; set; }
        public DbSet<lorder> lorder { get; set; }
        public DbSet<order_details> order_details { get; set; }
        public DbSet<kd> kd { get; set; }
        public DbSet<news> news { get; set; }
        public DbSet<product> product { get; set; }
        public DbSet<sms> sms { get; set; }
        public DbSet<wx_keyword> wx_keyword { get; set; }
        public DbSet<video_record> video_record { get; set; }
        public DbSet<zan> zan { get; set; }
    }
}

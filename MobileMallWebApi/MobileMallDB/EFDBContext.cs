using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using MobileMallDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB
{
    /// <summary>
    /// EFCore 数据库上下文
    /// </summary>
    public class EFDBContext : DbContext
    {
        public EFDBContext(DbContextOptions<EFDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Captcha> Captchas { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<Goods_Category> Goods_Categorys { get; set; }
        public DbSet<Goods_File> Goods_Files { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        #region 配置
        ///// <summary>
        ///// 视图模型

        ///// </summary>
        //public DbSet<OrderinfoView> OrderinfoView { get; set; }

        /// <summary>
        /// 在MobileMallDB 类库上执行迁移需要添加
        /// </summary>
        public class StoreDbContexttFactory : IDesignTimeDbContextFactory<EFDBContext>
        {
            public EFDBContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<EFDBContext>();
                optionsBuilder.UseMySql("Persist Security Info=False;Database=MobileMallDB;Data Source=localhost;User Id=root;Password=li2442;port=3306;charset=utf8mb4;", new MySqlServerVersion("8.4.0"));
                return new EFDBContext(optionsBuilder.Options);
            }
            // 使用 Scoped 每次调用ef 都创建新的实例防止占用
            // EFCore数据库表的创建：
            // 1、调用EF自带方法创建，需要先删除库再重新创建库（Context.Database.EnsureCreate()）
            // 2、项目目录CMD执行迁移文件命令，此操作可以用来生成库也可以用来修改模型时更新数据库。
            //     （需要先在nuget包中安装：Microsoft.EntityFrameworkCore.Tools、Pomelo.EntityFrameworkCore.MySql）
            //     （没有安装dotnet-ef命令先安装一下：dotnet tool install --global dotnet-ef --version 7.0.20）
            //     1、 dotnet ef (用于检查是否安装ef命令)
            //     2、生成迁移文件（Migrations文件夹及里面的文件）： dotnet ef migrations add Migration1 -c EFDBContext ; 此操作会在项目目录下创建Migrations文件夹并加入数据文件（Migration1 指的本次迁移的名称）
            //     3、执行迁移文件（生成数据库表到mysql中）： dotnet ef database update -c EFDBContext    将应用新的迁移并更新数据库结构以反映你对实体类模型所做的更改。
        }

        /// <summary>
        /// 重写OnModelCreating，
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<OrderinfoView>()
            //            .HasNoKey()
            //            .ToView("OrderinfoView"); // 指定视图的名称（告诉 EF Core，这个实体类对应的是一个数据库视图，而不是一个表。）

            modelBuilder.Entity<User>()
                        .Property(e => e.user_id)
                        .ValueGeneratedOnAdd();
            modelBuilder.Entity<User>()
                        .HasIndex(e => e.nick_name) // 指定要添加索引的属性
                        .HasDatabaseName("Index_Nickname"); // 可选：指定索引名称

            modelBuilder.Entity<Goods>()
                       .Property(e => e.goods_id)
                       .ValueGeneratedOnAdd();
            modelBuilder.Entity<Goods>()
                       .HasIndex(e => e.goods_name) // 指定要添加索引的属性
                       .HasDatabaseName("Index_GoodsName"); // 可选：指定索引名称

            modelBuilder.Entity<Goods>()
               .HasOne(c => c.Goods_Categorys)
               .WithMany(s => s.Goods)
               .HasForeignKey(s => s.category_id)
               .OnDelete(DeleteBehavior.Cascade);// 设置级联删除
                                                 // DeleteBehavior:关联删除通常是一个数据库术语，用于描述在删除行时允许自动触发删除关联行的特征；即当主表的数据行被删除时，自动将关联表中依赖的数据行进行删除，或者将外键更新为NULL或默认值。
                                                 // 1、ON DELETE NO ACTION: 默认行为，删除主表数据行时，依赖表中的数据不会执行任何操作，此时会产生错误，并回滚DELETE语句。
                                                 // 2、ON DELETE CASCADE: 删除主表数据行时，依赖表的中数据行也会同步删除。
                                                 // 3、ON DELETE SET NULL: 删除主表数据行时，将依赖表中数据行的外键更新为NULL。为了满足此约束，目标表的外键列必须可为空值。
                                                 // 4、ON DELETE SET DEFAULT: 删除主表数据行时，将依赖表的中数据行的外键更新为默认值。为了满足此约束，目标表的所有外键列必须具有默认值定义；如果外键可为空值，并且未显式设置默认值，则将使用NULL作为该列的隐式默认值。

            modelBuilder.Entity<Goods_File>()
                .HasOne(c => c.Goods)
                .WithMany(s => s.Goods_Files)
                .HasForeignKey(s => s.goods_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Users)
                .WithMany(s => s.Comments)
                .HasForeignKey(s => s.user_id)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Goods)
                .WithMany(s => s.Comments)
                .HasForeignKey(s => s.goods_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
               .HasOne(c => c.Goods)
               .WithMany(s => s.Carts)
               .HasForeignKey(s => s.goods_id)
               .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cart>()
               .HasOne(c => c.Users)
               .WithMany(s => s.Carts)
               .HasForeignKey(s => s.user_id)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .HasOne(c => c.Users)
               .WithMany(s => s.Orders)
               .HasForeignKey(s => s.user_id)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetails>()
              .HasOne(c => c.Orders)
              .WithMany(s => s.OrderDetails)
              .HasForeignKey(s => s.order_id)
              .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderDetails>()
              .HasOne(c => c.Goods)
              .WithMany(s => s.OrderDetails)
              .HasForeignKey(s => s.goods_id)
              .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileMallDB.BusinessInterface;
using MobileMallDB.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB
{
    public static class EFDBContextMiddlewareExtensions
    {
        /// <summary>
        /// 使用日志中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static void UseEFDBContextMiddlewareExtensions(this IServiceCollection app,string connectionString)
        {
            #region EFCore
            // 1、注册 EFDBCOntext
            app.AddDbContext<EFDBContext>(options =>
            {
                //string connectionString = builder.Configuration.GetConnectionString("DBConnectionStr");
                // Persist Security Info=False;Database=hospitalselfserver_sami;Data Source=localhost;User Id=root;Password=li2442;port=3306;charset=utf8mb4;SslMode=none;
                options.UseMySql(connectionString, new MySqlServerVersion("8.4.0")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                // UseQueryTrackingBehavior方法用于设置查询跟踪行为。
                //      1、QueryTrackingBehavior.TrackAll：EF Core将会记录所有的查询，包括查询结果。
                //      2、QueryTrackingBehavior.NoTracking：EF Core将不会跟踪任何实体实例，也不会有任何实体实例会被自动附加到上下文中。
                //      3、QueryTrackingBehavior.IdentityOnly：EF Core只会跟踪那些被用作数据库的键的实体。

                //options.LogTo(Console.WriteLine, LogLevel.Information);
                //options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Scoped); // 使用 Scoped 每次调用ef 都创建新的实例防止占用
                                        // EFCore数据库表的创建：
                                        // 1、调用EF自带方法创建，需要先删除库再重新创建库（Context.Database.EnsureCreate()）
                                        // 2、项目目录CMD执行迁移文件命令，此操作可以用来生成库也可以用来修改模型时更新数据库。
                                        //     （需要先在nuget包中安装：Microsoft.EntityFrameworkCore.Tools、Pomelo.EntityFrameworkCore.MySql）
                                        //     （没有安装dotnet-ef命令先安装一下：dotnet tool install --global dotnet-ef --version 7.0.20）
                                        //     1、 dotnet ef (用于检查是否安装ef命令)
                                        //     2、生成迁移文件（Migrations文件夹及里面的文件）： dotnet ef migrations add Migration1 -c EFDBContext ; 此操作会在项目目录下创建Migrations文件夹并加入数据文件（Migration1 指的本次迁移的名称）
                                        //     3、执行迁移文件（生成数据库表到mysql中）： dotnet ef database update -c EFDBContext    将应用新的迁移并更新数据库结构以反映你对实体类模型所做的更改。

            app.AddScoped<IBaseService, BaseService>();
            //builder.Services.AddSingleton<IProductService, ProductService>();

            #endregion
        }
    }
}

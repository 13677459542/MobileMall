using Microsoft.OpenApi.Models;
using System.Reflection;
using MobileMallWebApi.Utils;
using MobileMallWebApi.Utils.Authorization;
using MobileMallWebApi.Utils.LogMiddleware;
using MobileMallDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc;
using MobileMallWebApi.ApiModels.Resp;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//定义接口返回值传输的时间格式(先在Nuget引用Microsoft.AspNetCore.Mvc.NewtonsoftJson)
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 1、添加中间件支持跨域
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*") // 替换为允许访问的域名，*代表允许所有域请求
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowAnyOrigin();
        //.AllowCredentials();
    });
});

//模型绑定 特性验证，自定义返回格式
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        //获取验证失败的模型字段 
        var errors = actionContext.ModelState
        .Where(e => e.Value.Errors.Count > 0)
        .Select(e => e.Value.Errors.First().ErrorMessage)
        .ToList();
        //var str = string.Join("|", errors);
        var str = errors.FirstOrDefault();
        //设置返回内容
        var result = new BaseResp<string>()
        {
            status = 209,
            message = str,
            data = ""
        };
        // 返回自定义的状态码和内容
        return new ObjectResult(result)
        {
            StatusCode = 209 // 设置自定义状态码
        };
        //return new BadRequestObjectResult(result);
    };
});

builder.Services.AddHttpClient();// 将IHttpClientFactory和相关服务添加到IServiceCollection。

builder.UseSerilogExtensions();// 配置Serilog日志

builder.Services.UseJWTMiddlewareExtensions(builder); // 注入JWT扩展

builder.Services.AddSwaggerGen(options =>
{
    options.UseSwaggerJoinBearer();// Swagger配置支持Token参数传递

    //options.SwaggerDoc("v1", new OpenApiInfo
    //{
    //    Title = "WebApiDemo接口",
    //    Version = "V1.0.0",
    //    Description = "WebApiDemo_WebAPI"
    //});

    #region 接口版本分组（第一步）
    foreach (FieldInfo field in typeof(ApiVersioninfo).GetFields())
    {
        options.SwaggerDoc(field.Name, new OpenApiInfo()
        {
            Title = $"{field.Name}版本 dotnet core webapi",
            Version = field.Name,
            Description = $"{field.Name}版本"
        });
    }
    #endregion

    #region 显示接口注释信息
    var file = Path.Combine(AppContext.BaseDirectory, "MobileMallWebApi.xml");  // xml文档绝对路径，文件名称设置为项目名称
    var path = Path.Combine(AppContext.BaseDirectory, file); // xml文档绝对路径，需要先在 项目右键属 > 生成 > 文档文件 > 勾选生成包含API文档的文件
    options.IncludeXmlComments(path, true); // true : 显示控制器层注释
    #endregion
    options.OrderActionsBy(o => o.RelativePath); // 对action的名称进行排序，如果有多个，就可以看见效果了。
});

builder.Services.UseEFDBContextMiddlewareExtensions(builder.Configuration.GetConnectionString("DBConnectionStr")); // 注入 EF Core 扩展

builder.Services.AddHttpContextAccessor();// 为lHttpContextAccessor服务添加默认实现。

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    #region 接口版本分组（第二步）
    app.UseSwaggerUI(options =>
    {
        foreach (FieldInfo field in typeof(ApiVersioninfo).GetFields())
            options.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"版本选择：{field.Name}");
    });
    #endregion
}

app.UseRequestResponseLogging();// 使用日志中间件

app.UseCors(); // 2、跨域中间件生效

app.UseStaticFiles();// 启用静态文件服务,这是 ASP.NET Core 应用程序中默认的静态文件目录。当你启用静态文件服务时，只有 wwwroot 目录中的文件会被公开访问。
// 启用其他目录中的静态文件
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticDataSource")),
    RequestPath = "/StaticDataSource"
});

app.UseHttpsRedirection();// 添加中间件以将HTTP请求重定向到HTTPS
app.UseAuthentication();// 鉴权 启用JWT Token认证 注意顺序一定是先认证(UseAuthentication)后授权(UseAuthorization) 不然接口即使附加token也认证不通过
app.UseAuthorization();// 授权

app.UseMiddleware<AesEncryptionMiddleware>();// 配置请求/响应AES加解密中间件

app.MapControllers();

app.Run();

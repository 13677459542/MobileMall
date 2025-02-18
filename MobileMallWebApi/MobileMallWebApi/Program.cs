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
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//����ӿڷ���ֵ�����ʱ���ʽ(����Nuget����Microsoft.AspNetCore.Mvc.NewtonsoftJson)
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// 1������м��֧�ֿ���
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*") // �滻Ϊ������ʵ�������*������������������
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowAnyOrigin();
        //.AllowCredentials();
    });
});

//ģ�Ͱ� ������֤���Զ��巵�ظ�ʽ
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        //��ȡ��֤ʧ�ܵ�ģ���ֶ� 
        var errors = actionContext.ModelState
        .Where(e => e.Value.Errors.Count > 0)
        .Select(e => e.Value.Errors.First().ErrorMessage)
        .ToList();
        //var str = string.Join("|", errors);
        var str = errors.FirstOrDefault();
        //���÷�������
        var result = new BaseResp<string>()
        {
            status = 209,
            message = str,
            data = ""
        };
        // �����Զ����״̬�������
        return new ObjectResult(result)
        {
            StatusCode = 209 // �����Զ���״̬��
        };
        //return new BadRequestObjectResult(result);
    };
});

builder.Services.AddHttpClient();// ��IHttpClientFactory����ط�����ӵ�IServiceCollection��

builder.UseSerilogExtensions();// ����Serilog��־

builder.Services.UseJWTMiddlewareExtensions(builder); // ע��JWT��չ

builder.Services.AddSwaggerGen(options =>
{
    options.UseSwaggerJoinBearer();// Swagger����֧��Token��������

    //options.SwaggerDoc("v1", new OpenApiInfo
    //{
    //    Title = "WebApiDemo�ӿ�",
    //    Version = "V1.0.0",
    //    Description = "WebApiDemo_WebAPI"
    //});

    #region �ӿڰ汾���飨��һ����
    foreach (FieldInfo field in typeof(ApiVersioninfo).GetFields())
    {
        options.SwaggerDoc(field.Name, new OpenApiInfo()
        {
            Title = $"{field.Name}�汾 dotnet core webapi",
            Version = field.Name,
            Description = $"{field.Name}�汾"
        });
    }
    #endregion

    #region ��ʾ�ӿ�ע����Ϣ
    var file = Path.Combine(AppContext.BaseDirectory, "MobileMallWebApi.xml");  // xml�ĵ�����·�����ļ���������Ϊ��Ŀ����
    var path = Path.Combine(AppContext.BaseDirectory, file); // xml�ĵ�����·������Ҫ���� ��Ŀ�Ҽ��� > ���� > �ĵ��ļ� > ��ѡ���ɰ���API�ĵ����ļ�
    options.IncludeXmlComments(path, true); // true : ��ʾ��������ע��
    #endregion
    options.OrderActionsBy(o => o.RelativePath); // ��action�����ƽ�����������ж�����Ϳ��Կ���Ч���ˡ�
});

builder.Services.UseEFDBContextMiddlewareExtensions(builder.Configuration.GetConnectionString("DBConnectionStr")); // ע�� EF Core ��չ

builder.Services.AddHttpContextAccessor();// ΪlHttpContextAccessor�������Ĭ��ʵ�֡�

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    #region �ӿڰ汾���飨�ڶ�����
    app.UseSwaggerUI(options =>
    {
        foreach (FieldInfo field in typeof(ApiVersioninfo).GetFields())
            options.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"�汾ѡ��{field.Name}");
    });
    #endregion
}

app.UseRequestResponseLogging();// ʹ����־�м��

app.UseCors(); // 2�������м����Ч

app.UseStaticFiles();// ���þ�̬�ļ�����,���� ASP.NET Core Ӧ�ó�����Ĭ�ϵľ�̬�ļ�Ŀ¼���������þ�̬�ļ�����ʱ��ֻ�� wwwroot Ŀ¼�е��ļ��ᱻ�������ʡ�
// ��������Ŀ¼�еľ�̬�ļ�
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticDataSource")),
    RequestPath = "/StaticDataSource"
});

app.UseHttpsRedirection();// ����м���Խ�HTTP�����ض���HTTPS
app.UseAuthentication();// ��Ȩ ����JWT Token��֤ ע��˳��һ��������֤(UseAuthentication)����Ȩ(UseAuthorization) ��Ȼ�ӿڼ�ʹ����tokenҲ��֤��ͨ��
app.UseAuthorization();// ��Ȩ

app.UseMiddleware<AesEncryptionMiddleware>();// ��������/��ӦAES�ӽ����м��

app.MapControllers();

app.Run();

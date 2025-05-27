using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileMallWebApi.ApiModels.Resp;
using MobileMallWebApi.Utils.Encryption;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MobileMallWebApi.Utils.Authorization
{
    /// <summary>
    /// 请求/响应中间件
    /// </summary>
    public class AesEncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public AesEncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var allowAnonymous = context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null; // 使用反射获取当前请求的 endpoint 信息，确认当前请求接口是否运行匿名访问

            var NotVerify = !context.Request.Path.Value.ToLower().Contains("/authorization/");

            // 只对POST请求进行解密
            if (context.Request.Method == HttpMethods.Post && NotVerify)
            {
                context.Request.EnableBuffering(); // 读取请求体
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var encryptedBody = await reader.ReadToEndAsync(); // 读取请求入参
                    context.Request.Body.Position = 0; // 重置流位置
                    if (!string.IsNullOrEmpty(encryptedBody))
                    {
                        JObject jsonObject = JObject.Parse(encryptedBody);
                        // 解密请求体
                        try
                        {
                            string BodyStr = (string)jsonObject["EncryptedData"];
                            // 使用示例
                            if (TryGetHexBytes(jsonObject, "KEY", "IV", out byte[] currentKey, out byte[] currentIv))
                            {
                                //byte[] currentKey = Convert.FromHexString((string)jsonObject["KEY"]);
                                //byte[] currentIv = Convert.FromHexString((string)jsonObject["IV"]);

                                if (!string.IsNullOrEmpty(BodyStr))
                                {
                                    string decryptedBody = AesEncryption.DecryptStringFromBytes_Aes(BodyStr, currentKey, currentIv); // 使用您的解密逻辑
                                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(decryptedBody)); // 将解密内容赋值给请求体
                                }
                                // 存储到当前请求的上下文，供后续反参加密使用
                                context.Items["AesKey"] = currentKey;
                                context.Items["AesIv"] = currentIv;
                            }
                            else
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest; // 处理解密失败
                                await context.Response.WriteAsync($"{{\"status\":400,\"message\":\"密钥有误，请检查!\"}}"); // 返回错误信息
                                return; // 确保不继续处理请求
                            }

                        }
                        catch (Exception ex)
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError; // 处理解密失败
                            await context.Response.WriteAsync($"{{\"status\":500,\"message\":\"解密失败!{ex.Message}\"}}"); // 返回错误信息
                            return; // 确保不继续处理请求
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest; // 处理解密失败
                        await context.Response.WriteAsync($"{{\"status\":400,\"message\":\"入参不可为空，请检查!\"}}"); // 返回错误信息
                        return; // 确保不继续处理请求
                    }
                }
            }

            // 调用下一个中间件
            await _next(context);

            // 只对成功的响应进行加密
            //if (context.Request.Method == HttpMethods.Post && context.Response.StatusCode == StatusCodes.Status200OK)
            if (context.Request.Method == HttpMethods.Post && NotVerify)
            {
                try
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync(); // 读取反参
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // 从当前请求的上下文取出密钥和IV
                    byte[] ResponseKey = (byte[])context.Items["AesKey"];
                    byte[] ResponseIv = (byte[])context.Items["AesIv"];

                    var AesStr = AesEncryption.EncryptStringToBytes_Aes(responseBody, ResponseKey, ResponseIv); // 加密响应数据
                    context.Response.ContentType = "application/json";  // 设置响应内容类型
                                                                        //await context.Response.WriteAsync($"{{\"EncryptedData\":\"{AesStr}\",\"IV\":\"{Convert.ToBase64String(AesEncryption.iv)}\"}}"); // 设置响应内容
                    await context.Response.WriteAsync(AesStr); // 设置响应内容
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"{{\"status\":500,\"message\":\"响应过程异常!{ex.Message}\"}}"); // 返回错误信息
                    return;
                }

            }
        }

        /// <summary>
        /// 检测传入的key和iv并使用RSA2进行解密
        /// </summary>
        /// <param name="json">json对象</param>
        /// <param name="key">检测的键</param>
        /// <param name="result">返回的字节数组</param>
        /// <returns></returns>
        private static bool TryGetHexBytes(JObject json, string key, string iv, out byte[] keyResult, out byte[] ivResult)
        {
            keyResult = null;
            ivResult = null;
            if (!json.TryGetValue(key, out JToken keytoken) || !json.TryGetValue(iv, out JToken ivtoken)) return false;
            if (keytoken.Type != JTokenType.String || ivtoken.Type != JTokenType.String) return false;
            try
            {
                var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8);
                string keyStr = rsa.Decrypt((string)keytoken);
                string ivStr = rsa.Decrypt((string)ivtoken);
                keyResult = Convert.FromHexString(keyStr);
                ivResult = Convert.FromHexString(ivStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

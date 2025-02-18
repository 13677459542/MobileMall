namespace MobileMallWebApi.Utils.Other
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class MethodTool
    {
        /// <summary>
        /// 获取当前服务的地址
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetServerPath(HttpContext httpContext)
        {
            // 获取请求的协议（http 或 https）
            var scheme = httpContext.Request.Scheme;
            // 获取客户端 IP 地址
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            // 获取端口号
            var port = httpContext.Connection.LocalPort;
            return $"{scheme}://{(ipAddress == "::1" ? "127.0.0.1" : ipAddress)}:{port}";
        }

        /// <summary>
        /// 手机号脱敏
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string MaskPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Length != 11) // 确保手机号长度为11位
                return phoneNumber;
            // 用 * 替换中间4位
            string masked = phoneNumber.Substring(0, 3) + "****" + phoneNumber.Substring(7);
            return masked;
        }
    }
}

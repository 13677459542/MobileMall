
using MobileMallWebApi.ApiModels.Resp;
using Newtonsoft.Json;

namespace MobileMallWebApi.Utils
{
    /// <summary>
    /// API数据返回工具类
    /// </summary>
    public class ApiResult
    {

        /// <summary>
        /// 成功响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static async Task<string> SetSuccess<T>(string msg, T t)
        {
            return JsonConvert.SerializeObject(new BaseResp<T>()
            {
                status = 200,
                message = "success",
                data = t
            });
        }

        /// <summary>
        /// 设置成功响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<string> SetSuccess<T>(T t, string msg) where T : List<T>
        {
            return JsonConvert.SerializeObject(new BaseResp<T>()
            {
                status = 200,
                message = msg,
                data = t
            });
        }

        /// <summary>
        /// 失败响应
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<string> SetFailure<T>(string msg, T data = default)
        {
            return JsonConvert.SerializeObject(new BaseResp<T>()
            {
                status = 201,
                message = msg,
                data = data
            });
        }

        /// <summary>
        /// 异常响应
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<string> SetException(string msg, string data)
        {
            return JsonConvert.SerializeObject(new BaseResp<string>()
            {
                status = 202,
                message = msg,
                data = data
            });
        }

        ///// <summary>
        ///// 成功响应
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="msg"></param>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //public static async Task<BaseResp<T>> SetSuccess<T>(string msg, T t)
        //{
        //    return new BaseResp<T>
        //    {
        //        status = 200,
        //        message = msg,
        //        data = t
        //    };
        //}

        ///// <summary>
        ///// 设置成功响应
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public static async Task<BaseResp<T>> SetSuccess<T>(T t, string msg) where T : List<T>
        //{
        //    return new BaseResp<T>
        //    {
        //        status = 200,
        //        message = msg,
        //        data = t
        //    };
        //}

        ///// <summary>
        ///// 失败响应
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public static async Task<BaseResp<T>> SetFailure<T>(string msg, T data = default)
        //{
        //    return new BaseResp<T>
        //    {
        //        status = 201,
        //        message = msg,
        //        data = data
        //    };
        //}

        ///// <summary>
        ///// 异常响应
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public static async Task<BaseResp<string>> SetException(string msg, string data)
        //{
        //    return new BaseResp<string>
        //    {
        //        status = 202,
        //        message = msg,
        //        data = data
        //    };
        //}
    }
}

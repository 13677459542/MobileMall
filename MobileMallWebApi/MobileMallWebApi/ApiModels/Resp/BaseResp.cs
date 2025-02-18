namespace MobileMallWebApi.ApiModels.Resp
{
    /// <summary>
    /// 响应基类
    /// </summary>
    public class BaseResp<T>
    {
        /// <summary>
        /// 状态码 200:表示成功 其他：表示失败
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 结果值
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 响应集
        /// </summary>
        public T? data { get; set; }
    }
}

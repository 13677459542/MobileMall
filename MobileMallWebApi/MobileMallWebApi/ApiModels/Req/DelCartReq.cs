using System.ComponentModel.DataAnnotations;

namespace MobileMallWebApi.ApiModels.Req
{
    /// <summary>
    /// 删除购物车记录
    /// </summary>
    public class DelCartReq
    {
        /// <summary>
        /// 购物车记录id集合
        /// </summary>
        [Required(ErrorMessage = "cartIds是必填项")]
        public List<int> cartIds {  get; set; }
    }
}

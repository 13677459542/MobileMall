using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB.Models
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// 评论id
        /// </summary>
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int comment_id { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [Column(TypeName = "LONGTEXT")] // 这里可以根据需要选择 TEXT、LONGTEXT 或 MEDIUMTEXT
        public string content { get; set; }
        /// <summary>
        /// 评分： 1-10之间
        /// </summary>
        [Required]
        [Range(1, 10, ErrorMessage = "content Value must be between 1 and 10.")]
        public int score { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public int user_id { get; set; }
        /// <summary>
        /// 商品id
        /// </summary>
        [Required]
        public int goods_id { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME")] // 明确指定为 DATETIME 类型
        public DateTime create_time { get; set; }


        /// <summary>
        /// 外键导航属性
        /// </summary>
        public User Users { get; set; }
        public Goods Goods { get; set; }
    }
}

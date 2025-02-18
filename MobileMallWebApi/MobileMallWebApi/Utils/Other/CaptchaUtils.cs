using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace MobileMallWebApi.Utils.Other
{
    /// <summary>
    /// 验证码工具类
    /// </summary>
    public class CaptchaUtils
    {
        #region 方案一 
        /// <summary>
        /// 获取图形验证码图片(设置背景图)
        /// </summary>
        /// <param name="WidthValue">验证码图片宽度（不可超过474）</param>
        /// <param name="HeightValue">验证码图片高度（不可超过474）</param>
        /// <param name="codeNumber">随机数</param>
        /// <returns></returns>
        public static string GetCaptchaImage(int WidthValue, int HeightValue,string codeNumber)
        {
            try
            {
                //创建基础图像，宽*px，高*px
                Bitmap image = new Bitmap(WidthValue, HeightValue);

                //将基础图像和画家对象绑定
                Graphics graphics = Graphics.FromImage(image);

                //创建纹理刷
                Image TextTureImage = Image.FromFile("./StaticDataSource/Image/Utils/TextTure.jpg");
                TextureBrush textureBrush = new TextureBrush(TextTureImage);

                //创建一个矩形，矩形的左上顶点坐标为（0，0），右下顶点坐标为（300，100）
                Rectangle rectangle = new Rectangle(0, 0, WidthValue, HeightValue);

                //画家使用纹理刷，在基础图像上刷出一个矩形            
                graphics.FillRectangle(textureBrush, rectangle);

                //创建渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(WidthValue, HeightValue), Color.Blue, Color.DarkRed);

                //字体属性：微软雅黑,字号30,加粗，斜体
                Font font = new Font("Georgia", 30, (FontStyle.Bold | FontStyle.Italic));
                int x = 30;
                for (int i = 0; i < codeNumber.Length; i++)
                {
                    //画家使用渐变画刷书写字符code，字符字体为font,字符坐标为（x,30)
                    graphics.DrawString(codeNumber[i].ToString(), font, brush, x, 10);

                    //字符的横坐标需要增加，否则字符会叠在一起
                    x += 40;
                }

                //在验证码上画2根不同颜色的干扰线，需要4个点，2支笔
                Point p1 = new Point(0, 35);
                Point p2 = new Point(WidthValue, 45);
                Pen pen1 = new Pen(Color.Green, 2);//设置笔的颜色和宽度

                Point p3 = new Point(0, 25);
                Point p4 = new Point(WidthValue, 30);
                Pen pen2 = new Pen(Color.Gray, 2);

                //画家使用pen1,画第1条干扰线
                graphics.DrawLine(pen1, p1, p2);

                //画家使用pen2,画第2条干扰线
                graphics.DrawLine(pen2, p3, p4);

                MemoryStream stream = new MemoryStream();

                //完成绘制，将图像image以.png格式保存到内存流stream中
                image.Save(stream, ImageFormat.Png);

                //销毁graphics对象和笔对象
                graphics.Dispose();
                pen1.Dispose();
                pen2.Dispose();

                //往redis中存验证码字符串并设置过期时间，用于登录时校验验证码，此处省略。

                //将stream转为字节流数组返给前端
                // 将字节数组转换为Base64字符串
                string base64String = Convert.ToBase64String(stream.ToArray());
                return base64String;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #region 生成验证码图片方案二
        /// <summary>
        /// 获取图形验证码图片
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Bitmap GenerateCaptchaImage(string text)
        {
            int width = 208;
            int height = 65;
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            // 随机字体
            Font[] fonts = {
            new Font("Arial", 20, FontStyle.Bold),
            new Font("Verdana", 20, FontStyle.Bold),
            new Font("Comic Sans MS", 20, FontStyle.Bold)
        };

            Random random = new Random();
            for (int i = 0; i < text.Length; i++)
            {
                // 随机颜色
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                graphics.DrawString(text[i].ToString(), fonts[random.Next(fonts.Length)], new SolidBrush(color), 10 + i * 30, 20);
            }

            // 添加干扰线
            for (int i = 0; i < 5; i++)
            {
                Pen pen = new Pen(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)), 2);
                graphics.DrawLine(pen, random.Next(width), random.Next(height), random.Next(width), random.Next(height));
            }

            graphics.Dispose();
            return bitmap;
        }
        #endregion

        /// <summary>
        /// 获取指定位数随机数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomText(int length)
        {
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string chars = "0123456789";
            StringBuilder result = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

    }
}

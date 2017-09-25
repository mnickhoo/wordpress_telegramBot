

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using wordpressBot.Models;

namespace imageProcess
{
    class ImageMerge
    {
        public static Byte[] MergeService(int chatId , string name , string title)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Staffs.Where(s => s.chat_id == chatId.ToString()).FirstOrDefault(); 
            Image frontImage;
            try
            {
               
                //get orginal image 
                var orginal = Image.FromFile(@"C:\behcimaBot\behcima-staff\" + chatId + "\\" + chatId + ".jpg");
                Bitmap resize = new Bitmap(orginal, new Size(265 , 266)); //resize image to 265 * 66 
                resize.Save(@"C:\behcimaBot\behcima-staff\" + chatId + "\\" + chatId + "-resized.jpg"); //save resized image with name user-256 
                frontImage = Image.FromFile(@"C:\behcimaBot\behcima-staff\" + chatId + "\\" + chatId + "-resized.jpg"); //get resize image 
                //Bitmap frontBitmap = new Bitmap(frontImage);
                //frontBitmap.Save(@"D:\processor\behcima-staff\user-256.jpg");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Image frame;
            try
            {
                frame = Image.FromFile(@"C:\behcimaBot\behcima-staff\frame.png"); //get frame image
            }
            catch (Exception ex)
            {
                throw ex;
            }

            using (frame)
            {
                using (var bitmap = new Bitmap(500, 500))
                {
                    using (var canvas = Graphics.FromImage(bitmap))
                    {
                        
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic; //set quality 
                        canvas.DrawImage(frontImage,
                                        new Rectangle( 118 , 35 , 265 , 266)); //draw frontimage
                        canvas.DrawImage(frame,
                                         new Rectangle(0,
                                                       0,
                                                       500,
                                                       500),
                                         new Rectangle(0,
                                                       0,
                                                       frame.Width,
                                                       frame.Height), //draw background image
                                         GraphicsUnit.Pixel);
                        canvas.DrawString(name, new Font("IRANSans", 12), Brushes.DodgerBlue , 205, 340);
                        canvas.DrawString(title, new Font("IRANSans", 12), Brushes.OrangeRed, 205, 410);

                        canvas.Save();
                    }
                    try
                    {
                        //bitmap.Save(@"C:\behcimaBot\behcima-staff\final.jpeg",
                        //            System.Drawing.Imaging.ImageFormat.Jpeg);
                        bitmap.Save(@"C:\behcimaBot\behcima-staff\"+ chatId +"\\final.jpeg",
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        Byte[] data;

                        using (var memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, ImageFormat.Bmp);
                            data = memoryStream.ToArray();
                        }
                        return data; 

                    }
                    catch (Exception ex) {
                        throw ex ;
                    }
                }
            }
        }


    }
}

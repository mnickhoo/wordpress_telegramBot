using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot;
using TelegramBotAPI.Service;
using wordpressBot.Models; 

namespace wordpressBot.Controllers
{
    public class MainTestController : Controller
    {
        private static string _token = "421249334:AAH_-z--HieRXs2vjKYNwtup1EuVO9_BvM8";
        //TelegramBotClient API = new TelegramBotClient(_token);
        TelegramBotClient API = TelegramService.createAsync();
        ApplicationDbContext db = new ApplicationDbContext();


        // GET: MainTest
        public async Task<ActionResult>  Index()
        {
            var req = Request.InputStream;
            var responsString = new StreamReader(req).ReadToEnd();
            var update = JsonConvert.DeserializeObject<update>(responsString);
            var message = update.message;
            var chat = update.message.chat;

            long chatId = chat.id; //chat_Id
            string userName = chat.username; //user_name
            //get user by chatId if user has existed record in database 
            try
            {
                var user = db.Staffs.Where(s => s.chat_id == chatId.ToString()).FirstOrDefault();
                if (user == null) //means user not found with this chatId
                {
                    //find user by userName 
                    user = db.Staffs.Where(s => s.userName == userName).FirstOrDefault();
                    if (user != null)//means user founded with username
                    {
                        //set chat Id into user record and save change
                        user.chat_id = chatId.ToString();
                        db.SaveChanges();
                        await API.SendTextMessageAsync(chatId, "شروع دریافت عکس");
                        var resultProfileUser = await getProfileImageAndSave((int)chatId, _token);
                        Stream stream = new MemoryStream(ImageArray);
                        await TelegramService.UploadImage(stream, chatId, "تقدیم به به‌سیمایی عزیز");
                    }
                    else
                    {
                        //send message you are not authorized 
                    }

                }
                else //mean user found with chatId
                {

                    var resultProfileUser = await getProfileImageAndSave((int)chatId, _token);

                    Stream stream = new MemoryStream(ImageArray);
                    await TelegramService.UploadImage(stream, chatId, "تقدیم به به‌سیمایی عزیز");
                }
            }
            catch (Exception e)
            {
                await API.SendTextMessageAsync(chatId, e.Message + "\n" + e.StackTrace + "\n -inner--" + e.InnerException);
                throw e;
            }

            //167344742


            //var firstImage = userProfiles.Photos.First();
            //var FileString = firstImage[0].FilePath;
            //await API.SendPhotoAsync(167344742, userProfiles.Photos[0].First().FileId , "این تویی شیطون ؟");

            return View();
        }
        public async  Task<ActionResult> test()
        {
            await getProfileImageAndSave(167344742, _token); 
            return View(); 
        }

        public async Task<ActionResult> getChatId()
        {
            var req = Request.InputStream;
            var responsString = new StreamReader(req).ReadToEnd();
            var update = JsonConvert.DeserializeObject<update>(responsString);
            var message = update.message;
            var chat = update.message.chat;

            long chatId = chat.id; //chat_Id
            try
            {
                var botToken = "438879625:AAHxEEh6T38AFNvLRA4KJoel6EP8QxB83FI";
                TelegramBotClient api = new TelegramBotClient(botToken);
                await api.SendTextMessageAsync(chatId, "your chat id is : " + chatId.ToString()); 
            }
            catch (Exception e)
            {

                throw e;
            }
           

            return View(); 
        }
        public async Task<string> getProfileImageAndSave(int chatId , string token)
        {
            var userProfiles = await API.GetUserProfilePhotosAsync(chatId);
            var fileId = userProfiles.Photos[0][0].FileId; //get first profile image of user and file Id
            var getFile = await API.GetFileAsync(fileId); //get file with file Id
            var filePath = getFile.FilePath; //get file path 
            Directory.CreateDirectory(@"C:\behcimaBot\behcima-staff\" + chatId);
            var url = "https://api.telegram.org/file/bot"+ token +"/";
            var fullUrl = url += filePath;
            WebClient wb = new WebClient();
            wb.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.33 Safari/537.36");
            wb.DownloadFile(fullUrl, @"C:\behcimaBot\behcima-staff\" + chatId  + "\\" + chatId + ".jpg");
            return "success";
        }
        //public async Task<string> sendPhoto(int chatId)
        //{
        //    await API.SendPhotoAsync(chatId,  , "یه به سیمایی فعال");

        //    return ""; 

        //}
    }
}
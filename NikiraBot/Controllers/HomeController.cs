using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using wordpressBot.Models;
using System.IO;
using Telegram.Bot;
using Newtonsoft.Json;
using TelegramBotAPI.Service;
using System.Text.RegularExpressions;

namespace wordpressBot.Controllers
{
    public class HomeController : Controller
    {
        public  ActionResult index(HttpRequestBase request)
        {
            var reader = new StreamReader(Request.InputStream);
            var json = reader.ReadToEnd();
            return View();
        }

        [HttpPost]
        public JsonResult Index()
        {
            var req = Request.InputStream;
            var responsString = new StreamReader(req).ReadToEnd();
            var update = JsonConvert.DeserializeObject<update>(responsString);
            var message = update.message;
            var chat = update.message.chat;

            return new JsonResult();
        }

        public ActionResult main()
        {
            var pattern = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";
            Regex r = new Regex(pattern);
            Match m = r.Match("http://oxteam.com");
            var x = m.Success;
            WordpressService wp = new WordpressService();
            PostModel post = new PostModel()
            {
                title = "تستی می باشد",
                content = "این برای تسته دیگه"
            };
            wp.setChatId(167344742);
            wp.CreatePost(post, false, 0); // create post 
            return View();
        }
        public void createFile(string text, string chatId)
        {
            var path = "C:\\mehdi";
            var fileName = chatId + ".txt";
            var fullNameAndPath = Path.Combine(path, fileName);

            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!System.IO.File.Exists(fullNameAndPath))
            {
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(text);
                }
            }
        }
    }
}
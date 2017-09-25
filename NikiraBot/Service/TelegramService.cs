using wordpressBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Services.Description;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotAPI.Service
{
    public class TelegramService
    {
        //database context
        public static ApplicationDbContext db = new ApplicationDbContext();
        public static TelegramBotClient createAsync()
        {
            TelegramBotClient telegramApiToken = new TelegramBotClient("<YOUR_TELEGRAM_BOT_TOKEN>");
            return telegramApiToken;
        }

        public async static Task<string> SendMessage(long chatId, string text, KeyboardButton[] menue = null, InlineKeyboardButton[][] inlineButtons = null, bool defaultKeyboard = false, bool hideKeyboard = false)
        {
            //make api token async
            var api = createAsync();
            if (menue != null) //if menue not null means when send message to client show specific menue to user
            {
                var replyMarkup = CreateKeyboardButton(menue);
                var result = await api.SendTextMessageAsync(chatId, text, false, false, 0, replyMarkup);
                return "success";
            }
            if (defaultKeyboard)
            {
                var btn = new KeyboardButton[]
                {
                   new KeyboardButton { Text = "تنظیمات ⚙️" } , new KeyboardButton {Text = "راهنما 💬" } /*, new KeyboardButton {Text = "منوی اصلی 🔰" }*/ , new KeyboardButton {Text = "ایجاد پست 🖌" }
                };

                ReplyKeyboardMarkup rkb = CreateKeyboardButton(btn);
                var result = await api.SendTextMessageAsync(chatId, text, false, false, 0, rkb);
                return "success";
            }
            if (inlineButtons != null)
            {
                InlineKeyboardMarkup inline = new InlineKeyboardMarkup(inlineButtons);
                var result = await api.SendTextMessageAsync(chatId, text, false, false, 0, inline);
            }
            if (hideKeyboard)
            {
                ReplyKeyboardHide replyKeboardHide = new ReplyKeyboardHide();
                var reply = await api.SendTextMessageAsync(chatId, text, false, false, 0, replyMarkup: replyKeboardHide);
            }
            else
            {
                var x = await api.SendTextMessageAsync(chatId, text);
            }
            return "success";
        }


        public static long GetChanellChatId(long chatId)
        {
            var chanelChatId = db.WordpressBotModel.Where(t => t.chatId == chatId).FirstOrDefault().chanellChatId;
            return chanelChatId;
        }
        public static bool hasRegisterd(long chatId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var User = db.WordpressBotModel.Where(t => t.chatId == chatId).FirstOrDefault();

            if (User != null)
            {
                if (User.chatId == chatId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //long chtId = 167344742;
            return false;
        }
        public static string hasOperationAndType(Telegram.Bot.Types.Message message)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.WordpressBotModel.Where(t => t.chatId == message.Chat.Id).First();
            var operation = user.operation;
            if (operation != null && (message.Text.StartsWith("/") || message.Text == "لغو ❌"))
            {
                user.operation = null;
                db.SaveChanges();
                return null;
            }
            return operation;
        }

        public async static Task<string> UploadImage(HttpPostedFileBase image, long chatId, string caption)
        {
            var api = createAsync(); //create APi
            var fileName = image.FileName; // file Name of image
            var fts = new FileToSend(fileName, image.InputStream);

            // send image to chanel 
            await api.SendPhotoAsync(chatId, fts, caption);
            return "1";
        }
        public async static Task<string> UploadImage(Stream image, long chatId, string caption)
        {
            var api = createAsync(); //create APi
            var fts = new FileToSend("image", image);

            // send image to chanel 
            await api.SendPhotoAsync(chatId, fts, caption);
            return "1";
        }

        public static ReplyKeyboardMarkup CreateKeyboardButton(KeyboardButton[] keyboards)
        {
            const int row = 2;
            //const int rowNumber = row; 
            KeyboardButton[][] kb = new KeyboardButton[row][]
                {
                    keyboards ,
                    new KeyboardButton[] { new KeyboardButton {Text = "لغو ❌" } , new KeyboardButton { Text = "بازگشت" } } ,
                };
            ReplyKeyboardMarkup rkb = new ReplyKeyboardMarkup(kb);
            return rkb;
        }
        public static async Task<string> CalbackAnswer(string calbackId)
        {
            var Api = createAsync();
            InlineQueryResult[] inlines = new InlineQueryResult[]
            {
                new InlineQueryResult { Title = "همینه" }
            };
            await Api.AnswerCallbackQueryAsync(calbackId , "جوجه اردک زشت");
            return "success";
        }

    }
}
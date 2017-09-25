using Newtonsoft.Json;
using wordpressBot.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramBotAPI.Service;

namespace wordpressBot.Controllers
{
    public class ServiceController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Index()
        {
            var req = Request.InputStream; //send Request from telegram 
            var responsString = new StreamReader(req).ReadToEnd(); //read request 
            var update = JsonConvert.DeserializeObject<Update>(responsString); // deserialize to update model
            var message = update.Message; //get message entity
            var chat = message.Chat; //get chat entity

            try
            {
                if (update.CallbackQuery != null)
                {
                    await TelegramService.SendMessage(chat.Id, "جان سلام" + update.CallbackQuery.Data);
                }

                InlineKeyboardButton[][] inline = new InlineKeyboardButton[][]
                {
                         new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "سلام"  , CallbackData = "hi" , SwitchInlineQuery = "" } }
                };
                await TelegramService.SendMessage(167344742, "salam", null, inline);
            }
            catch (Exception e)
            {
                await TelegramService.SendMessage(chat.Id, e.Message);
            }

            if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data == "hi")
                {
                    await TelegramService.CalbackAnswer(update.CallbackQuery.Id);
                }
                //await TelegramService.CalbackAnswer(update.InlineQuery.Id);
            }

            ApplicationDbContext db = new ApplicationDbContext();//database context 
          
            #region Comment Inline KeyBoard Buttons
            //InlineKeyboardButton[][] buttons = new InlineKeyboardButton[1][]
            //{
            //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "", Url = "" } }
            //};

            //InlineKeyboardButton[][] buttons1 = new InlineKeyboardButton[][]
            //{
            //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "سلام", Url = "nickhoo.ir" } }  , 
            //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "سلام", Url = "nickhoo.ir" } }  ,
            //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "سلام", Url = "nickhoo.ir" } }  ,
            //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "سلام", Url = "nickhoo.ir" } }  ,
            //};
            //int i = 0;
            //foreach (var post in posts)
            //{
            //    buttons[i] = new InlineKeyboardButton[] { new InlineKeyboardButton { Text = post.title, Url = post.url } };
            //    i++;
            //}

            //InlineKeyboardMarkup inline = new InlineKeyboardMarkup(buttons);
            #endregion

            var hasRegistered = TelegramService.hasRegisterd(chat.Id); //get owner chatId for respons
            if (hasRegistered) //if user registered login and use with bot else user register in databse and setup bot for wordpress using
            {
                var ownerChatID = chat.Id; 
                var user = db.WordpressBotModel.Where(t => t.chatId == ownerChatID).FirstOrDefault(); //get user with chatId 
                var hasOperation = TelegramService.hasOperationAndType(message); //check user have operation for bot processing or not !
                if (hasOperation != null)
                {
                    switch (hasOperation) //if user have operation check witch operation should be processing
                    {
                        case "/siteAddress": //if operan be siteAddress means user send add site and operation wait for input user and check user valid address or not
                            var pattern = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";
                            Regex r = new Regex(pattern); //regular expression using for check input has a valid address or not
                            Match m = r.Match(message.Text); //match with user input 
                            if (m.Success) // if successfull
                            {
                                user.siteAddress = message.Text; //add user input to database
                                user.operation = null; // null operation means operation accomplishment
                                db.SaveChanges(); //finally save change in database
                                //send message to user opearion done
                                await TelegramService.SendMessage(ownerChatID, "( آدرس سایت با موفقیت دریافت شد حالا با کلیک بر روی نام کاربری سایت وردپرسی خود را وارد کنید برای تنظیمات الزامی است");
                                break;
                            } //else if invalid address send by user send a message from bot that address invalid
                            await TelegramService.SendMessage(ownerChatID, "❌ آدرس وارد شده صحیح نمی باشد \n لطفا به همراه http یا https وارد کنید");
                            break;
                        case "/wpUser":
                            user.wpUser = message.Text;
                            user.operation = null;
                            db.SaveChanges();
                            await TelegramService.SendMessage(ownerChatID, "! نام کاربری با موفقیت ذخیره شد  \n با زدن بر روی /password میتوانید پسورد خود را ثبت یا ویرایش کنید");
                            break;
                        case "/wpPassword":
                            user.wpPassword = message.Text;
                            user.operation = null;
                            db.SaveChanges();
                            await TelegramService.SendMessage(ownerChatID, "خیر خب تنظیمات سایت وردپرسی شما تمام شد برای وارد کردن نام کاربری کانال  /userchanell بزنید");
                            break;
                        case "/userchanell":
                            if (message.Text.StartsWith("@"))
                            {
                                user.chanellUser = message.Text;
                                user.operation = null;
                                db.SaveChanges();
                                await TelegramService.SendMessage(ownerChatID, "نام کاربری کانال با موفقیت دریافت شد");
                                break;
                            }
                            await TelegramService.SendMessage(ownerChatID, "❌ نام کاربری وارد شده صحیح نمی باشد \n نام کاربری باید با @ شروع بشود ");
                            break;
                        case "/title":
                            user.title = message.Text;
                            user.operation = "/description";
                            db.SaveChanges();
                            await TelegramService.SendMessage(ownerChatID, "عنوان دریافت شد ! \n حالا توضیحات خود را بنویسید ");
                            break;
                        case "/description":
                            user.description = message.Text;
                            user.operation = null;
                            db.SaveChanges();
                            var text = "خیر خب پست شما آماده است بر روی پیش نمایش کلیک کنید تا قبل از ارسال یه بررسی کنید";
                            var descriptionButton = new KeyboardButton[] { new KeyboardButton { Text = "پیش نمایش" }, new KeyboardButton { Text = "ویرایش مجدد" } };
                            await TelegramService.SendMessage(ownerChatID, text, descriptionButton);
                            break;
                    }// End Switch Block
                    return View();
                } //if hasOperation 
                else // if hasOperaion is null
                {
                    try
                    {
                        switch (message.Text)
                        {
                            case "راهنما 💬":
                            case "راهنما":
                            case "/help":
                                //InlineKeyboardButton[][] buttons = new InlineKeyboardButton[2][]
                                //{
                                //    new InlineKeyboardButton[] { new InlineKeyboardButton { Text = "اشتراک گذاری بات", SwitchInlineQuery = "وردپرس داری کانال هم که داری ؟ خب بیا من راهنماییت میکنم" } } ,
                                //    new InlineKeyboardButton[] {new InlineKeyboardButton { Text = "رمز عبور" , CallbackData = "رمز عبور" } , new InlineKeyboardButton {Text = "تنظیمات ⚙️" , CallbackData = "/setting" } }
                                //};
                                var menueText = "برای استفاده از بات شما میتوانید از دستورات زیر استفاده کنید \n" +
                                                                 "با زدن /help شما میتوانید راهنما از صفحه ی راهنما استفاده کنید \n" +
                                                                 "دستور /title برای ارسال عنوان پست  \n" +
                                                                 "دستور /description برای ارسال توضیحات پست \n" +
                                                                 "دستور /preview برای پیش نمایش پست جدید  \n" +
                                                                 "دستور /done با در نظر گرفتن تایید شما پست ارسال میشود \n" +
                                                                 "دستور /setting برای نمایش تنظیمات انجام شده \n" +
                                                                 "دستور /userchanell برای ارسال نام کاربری ربات \n" +
                                                                 "دستور /site برای ویرایش یا افزودن آدرس سایت \n" +
                                                                 "دستور /password برای ویرایش یا افزودن رمز عبور سایت \n" +
                                                                 "دستور /username برای ویرایش یا افزودن نام کاربری سایت \n";
                                await TelegramService.SendMessage(ownerChatID, menueText, null, null, true);
                                break;
                            case "آدرس سایت":
                            case "تغییر آدرس سایت":
                            case "/site":
                                user.operation = "/siteAddress";
                                db.SaveChanges();
                                await TelegramService.SendMessage(ownerChatID, "آدرس سایت وردپرسی خود را وارکنید خود را وارد کنید \n با http:// یا https:// شروع شود");
                                break;
                            case "رمز عبور":
                            case "تغییر رمز عبور":
                            case "/password":
                                user.operation = "/wpPassword";
                                db.SaveChanges();
                                await TelegramService.SendMessage(ownerChatID, "رمز عبور سایت وردپرسی خود را وارد کنید");
                                break;
                            case "نام کاربری":
                            case "تغییر نام کاربری":
                                user.operation = "/wpUser";
                                db.SaveChanges();
                                var messageUser = "نام کاربری فعلی" + user.wpUser + "\n اگر صحیح نمی باشد لطفا نام کاربری جدید را وارد کنید";
                                await TelegramService.SendMessage(ownerChatID, messageUser);
                                break;
                            case "تغییر نام کاربری کانال":
                            case "کانال":
                                user.operation = "/userchanell";
                                db.SaveChanges();
                                await TelegramService.SendMessage(ownerChatID, "نام کاربری کانال خود را وارد کنید \n مانند @nickhooir");
                                break;
                            case "/setting":
                            case "تنظیمات":
                            case "تنظیمات ⚙️":
                                var settingButton = new KeyboardButton[] { new KeyboardButton { Text = "تغییر رمز عبور" }, new KeyboardButton { Text = "تغییر نام کاربری" }, new KeyboardButton { Text = "تغییر آدرس سایت" }, new KeyboardButton { Text = "تغییر نام کاربری کانال" } };
                                var settingText = "🌐 تنظیمات سایت  \n آدرس سایت : " + user.siteAddress + "\n نام کاربری  : " + user.wpUser + "\n رمز عبور : " + user.wpPassword + "\n 📢 تنظیمات کانال \n نام کاربری کانال : " + user.chanellUser;
                                await TelegramService.SendMessage(ownerChatID, settingText, settingButton);
                                break;
                            case "/description":
                            case "توضیحات":
                                //var descriptionButton = new KeyboardButton[] { new KeyboardButton { Text = "پیش نمایش" } };
                                await TelegramService.SendMessage(ownerChatID, "توضیحات  پست خود را بنویسید یا ویرایش کنید");
                                user.operation = "/description";
                                db.SaveChanges();
                                break;
                            case "/title":
                            case "ویرایش مجدد":
                            case "ایجاد پست 🖌":
                                //var postButton = new KeyboardButton[] { new KeyboardButton { Text = "توضیحات" }};
                                await TelegramService.SendMessage(ownerChatID, "عنوان پست خود را بنویسید یا ویرایش کنید", hideKeyboard: true);
                                user.operation = "/title";
                                db.SaveChanges();
                                break;
                            case "بازگشت":
                                await TelegramService.SendMessage(ownerChatID, "یکی از موارد منو را انتخاب کنید", null, null, true);
                                break;
                            case "/done":
                            case "ارسال":
                                var ChanellChatId = TelegramService.GetChanellChatId(chat.Id);
                                await TelegramService.SendMessage(ChanellChatId, user.description); //send message in telegram chanell
                                PostModel post = new PostModel()
                                {
                                    title = user.title,
                                    content = user.description
                                };
                                WordpressService wp = new WordpressService();
                                wp.setChatId(chat.Id);
                                wp.CreatePost(post, false, 0); // create post 
                                await TelegramService.SendMessage(ownerChatID, " 🍀 با موفقیت ارسال شد", null, null, true); //send message in telegram chanell
                                user.title = null; //title to null
                                user.description = null; //description to null
                                db.SaveChanges();
                                break;
                            case "لغو ❌":
                                await TelegramService.SendMessage(ownerChatID, "دستور با موفقیت لغو شد", null, null, true);
                                break;
                            case "/preview":
                            case "پیش نمایش":
                                var txt = "🔸 عنوان در سایت : " + user.title + "\n ------- \n";
                                txt += user.description;
                                var previewButton = new KeyboardButton[] { new KeyboardButton { Text = "ارسال" }, new KeyboardButton { Text = "حذف" }, new KeyboardButton { Text = "ویرایش مجدد" } };
                                //InlineKeyboardButton[][] button = new InlineKeyboardButton[1][]
                                //{
                                //   new InlineKeyboardButton[] {new InlineKeyboardButton { Text = "لغو ارسال ❎", CallbackData = "/cancellSending" } , new InlineKeyboardButton {Text = " ارسال ✅", CallbackData = "/send" } }
                                //};
                                await TelegramService.SendMessage(ownerChatID, txt, previewButton);
                                db.SaveChanges();
                                break;
                            case "حذف":
                                user.title = null;
                                user.description = null;
                                db.SaveChanges();
                                await TelegramService.SendMessage(ownerChatID, "پست پیش نمایش با موفقیت حذف شد", null, null, true);
                                break;
                            default:
                                await TelegramService.SendMessage(ownerChatID, "موردی یافت نشد برای تعامل بهتر از منو استفاده کنید", null, null, true);
                                break;
                        }// End Switch Block
                        return View();
                    } //End Try Block
                    catch (Exception e)
                    {
                        var erorrMessage = e.InnerException.ToString();
                        var path = Server.MapPath("~/logs");
                        var timeStamp = ServiceController.GetTimestamp(DateTime.Now);
                        var fileName = timeStamp + ".txt";
                        var fullNameAndPath = Path.Combine(path, fileName);
                        using (StreamWriter sw = new StreamWriter(fullNameAndPath))
                        {
                            sw.WriteLine(e.InnerException);
                            sw.WriteLine(e.Message);
                            sw.WriteLine(e.StackTrace);
                            sw.WriteLine("+===================================================+");
                            sw.WriteLine(e.HelpLink);
                            sw.Close();
                        }
                        if (erorrMessage == "The remote server returned an error: (401) Unauthorized.")
                        {
                            await TelegramService.SendMessage(chat.Id, "مشکل در تنظیمات حساب وردپرسی لطفا نام کاربری و یا رمز عبور خود را بررسی کنید");
                            return View();
                        }

                        await TelegramService.SendMessage(chat.Id, "با خطا مواجه شدم 🔴 \n" + "کد خطا : " + timeStamp);
                        return View();
                    }//End Catch Block
                }
            }// End if registered
            else
            {
                if (message.Text == "/start" || message.Text == "/Start")
                {
                    WordpressBotModel telegramUser = new WordpressBotModel()
                    {
                        userName = chat.Username,
                        chatId = chat.Id,
                        fullName = chat.FirstName + " " + chat.LastName
                    };
                    var data = db.WordpressBotModel.Add(telegramUser);
                    db.SaveChanges();
                    var introMessage = "سلام " + telegramUser.fullName + "\n از این که ما را برای سرویس دهی به خود انتخاب کرده ای ازتون تشکر میکنیم \n امیدواریم بهترین تجربه را با ما داشته باشید";
                    await TelegramService.SendMessage(chat.Id, introMessage);
                }
            }// End if user not registered
            return View();
            #region botServiceApi
            //if (image != null && image.ContentLength > 0)
            //{
            //    //recieve request
            //    var request = Request.Files[0];
            //    //get image folder
            //    string imagefolder = Server.MapPath("~/image");
            //    //get file name 
            //    string fileName = image.FileName;
            //    //get extension 
            //    string fileExtension = Path.GetExtension(fileName);
            //    //get full path 
            //    string fullPath = Path.Combine(imagefolder, "temp" + fileExtension);
            //    request.SaveAs(fullPath); // save in path 

            //    //Service telegram
            //    var telegram = await TelegramService.UploadImage(request, chatid, post.content);

            //    using (Stream fs = new FileStream(fullPath, FileMode.Open))
            //    {
            //        var imageId = await WordpressService.templateUpload(fs, fileName); //get image id
            //        WordpressService.CreatePost(post, true, Convert.ToInt32(imageId)); //create post
            //    }
            //    return Json(new { sucess = true }, JsonRequestBehavior.AllowGet);
            //} // END if has image file 

            //    }
            //        else
            //        {
            //            var errors = ValidationService.GetModelStateErrors(ModelState);
            //            return Json(new { sucess = false, errors = errors
            //}, JsonRequestBehavior.AllowGet);
            //        } //END else if post has error 
            #endregion
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
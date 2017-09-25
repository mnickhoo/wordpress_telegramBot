using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telegram.Bot;

namespace NikiraBot.Service
{
    public class TelegramService
    {
        protected TelegramBotClient api { get { return getApi(); } }
        public TelegramBotClient getApi()
        {
            TelegramBotClient api = new TelegramBotClient("354883167:AAHm6GwhYkPm6MlMkLRI7j8nLI1yZ0WY6hY");
            return api;
        }
        public async Task<string> sendMessage(string chatId, string text)
        {
            //this section is hardcord
            await api.SendTextMessageAsync(167344742, text);
            return "success";
        }
    }
}
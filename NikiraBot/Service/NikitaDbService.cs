using wordpressBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace wordpressBot.Service
{
    public class wordpressBotDbService
    {
        /// <summary>
        /// get database context
        /// </summary>
        /// <returns></returns>
        protected static ApplicationDbContext getDb() 
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db;
        }
        private static ApplicationDbContext db = getDb();
        /// <summary>
        /// get user entity with chat id
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static WordpressBotModel getUserWithChatId(long chatId)
        {
            var user = db.WordpressBotModel.Where(t => t.chatId == chatId).FirstOrDefault();
            return user; 
        }
        public static string getSiteAddressByChatId(long chatId)
        {
            var siteAddress = db.WordpressBotModel.Where(t => t.chatId == chatId).FirstOrDefault().siteAddress;
            return siteAddress;
        }
    }
}
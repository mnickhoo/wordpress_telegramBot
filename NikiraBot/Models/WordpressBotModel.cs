using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace wordpressBot.Models
{
    public class WordpressBotModel
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public  long chatId { get; set; }
        public string operation { get; set; }
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$" ,  ErrorMessage = "آدرس سایت صحیح نمی باشد" )]
        public string siteAddress { get; set; }
        public string chanellUser { get; set; }
        public long chanellChatId { get; set; }
        public string wpUser { get; set; }
        public string wpPassword { get; set; }
        public string title { get; set; }
        public string description { get; set; }

    }

}
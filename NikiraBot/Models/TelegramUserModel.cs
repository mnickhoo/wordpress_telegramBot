using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndetitySample.Models
{
    public class TelegramUserModel
    {
        public int id { get; set; }
        public long chatId { get; set; }
        public string userName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wordpressBot.Models
{
    public class Staff
    {
        public int id { get; set; }
        public string name { get; set; }
        public Job jobtitle { get; set; }
        public string userName { get; set; }
        public string chat_id { get; set; }
    }

}

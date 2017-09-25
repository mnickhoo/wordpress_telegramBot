using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndetitySample.Models
{
    public class MediaModel
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string date_gmt { get; set; }
        public object guid { get;  }
        public string link { get;  }
        public string modified { get;  }
        public string modified_gmt { get;  }
        public string slug { get; set; }
        public string status { get; set; }
        public string type { get; }
        public object title { get; set; }
        public int author { get; set; }
        public string comment_status { get; set; }
        public string ping_status { get; set; }
        public object meta { get; set; }
        public string template { get; set; }
        public string alt_text { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        public string media_type { get;  }
        public string mime_type { get; }
        public object media_details { get; }
        public int post { get; set; }
        public Uri source_url { get; }

    }
}
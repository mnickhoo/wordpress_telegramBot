using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace wordpressBot.Models
{
    public class PostModel
    {
        public PostModel()
        {
            this.date = DateTime.Now;
        }
        public PostModel(string title = "این یک عنوانه" , string content = "این یک محتوا هستش")
        {
            this.title = title;
            this.content = content; 
        }

        public DateTime date { get; set; }
        public string date_gmt { get; set; }
        public string slug { get; set; }
        public string status { get; set; }
        public string password { get; set; }
        [Required( ErrorMessage = "این فیلد الزامی است" )]
        [MaxLength(50 , ErrorMessage = "حداکثر تا 50 کاراکتر الزامی است")]
        public string title { get; set; }
        //[Required(ErrorMessage = "فیلد محتوا الزامی می باشد")]
        [Required]
        public string content { get; set; }
        public string author { get; set; }
        public string excerpt { get; set; }
        public object featured_media { get; set; }
        public string comment_status { get; set; }
        public string ping_status { get; set; }
        public string format { get; set; }
        public string meta { get; set; }
        public string sticky { get; set; }
        public string template { get; set; }
        List<string> categories { get; set; }
        public string tags { get; set; }
        public string liveblog_likes { get; set; }
    }
}
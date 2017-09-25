using Newtonsoft.Json.Linq;
using wordpressBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TelegramBotAPI.Service
{
    public class WordpressService
    {
        private long _chatId; 
        public void setChatId(long chatId)
        {
            this._chatId = chatId; 
        }
        public WordpressBotModel GetUser()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.WordpressBotModel.Where(t => t.chatId == _chatId).FirstOrDefault();
            return user; 
        }
        public string GetSite()
        {
            var user = GetUser();
            return user.siteAddress; 
        }
        public  string GetApiUrl(string methode)
        {
            var Website = GetSite() + "/wp-json/wp/v2/";

            switch (methode)
            {
                case "posts":
                    return Website + methode;
                case "media":
                    return Website + methode;
                default:
                    return "no method select";
            }
        }

        // you should be athenticate bot with basic athentication in wordpress and install plugin
        //for more information using below article 
        //https://wordpress.org/plugins/wp-basic-auth/
        public string GetTokenBasic()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.WordpressBotModel.Where(t => t.chatId == _chatId).FirstOrDefault();
            var userName = user.wpUser;
            var passwrod = user.wpPassword;
            string userNameAndPassword = userName + ":" + passwrod; 
            var token = Convert.ToBase64String(Encoding.Default.GetBytes(userNameAndPassword));
            return "Basic " + token;
        }
        public string CreatePost(PostModel post, bool image, int imageId)
        {
            var request = (HttpWebRequest)WebRequest.Create(GetApiUrl("posts"));
            request.Headers["Authorization"] = GetTokenBasic();
            request.Method = "POST";
            request.UseDefaultCredentials = true;
            string postData;
            if (image)
            {
                postData = "title=" + post.title +
                            "&content=" + post.content + // Inclue HTML Markup
                            "&status=publish" +
                            "&date=" + TimeZone.CurrentTimeZone.ToUniversalTime(post.date).ToString("yyyy-MM-ddTHH:mm:ss") + "&featured_media=" + imageId;

            }
            else
            {
                postData = "title=" + post.title +
                            "&content=" + post.content + // Inclue HTML Markup
                            "&status=publish" +
                            "&date=" + TimeZone.CurrentTimeZone.ToUniversalTime(post.date).ToString("yyyy-MM-ddTHH:mm:ss") + "&featured_media=4946";
            }


            //set in Request option
            var data = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var json = JObject.Parse("{" + JObject.Parse(responseString)["guid"].First + "}");
                var x = "Post posted at " + json["rendered"].ToString();
                var y = json["rendered"].ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return "";
        }

        public  string UploadMedia(HttpPostedFileBase file, string path)
        {
            var request = (HttpWebRequest)WebRequest.Create(GetApiUrl("media"));
            request.Headers["Authorization"] = GetTokenBasic();
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UseDefaultCredentials = true;
            //MediaModel media = new MediaModel()
            //{
            //    title = "پست تستی",
            //    post = 2,
            //};

            //FileStream fs = new FileStream(path, FileMode.Open);
            char[] fileBytes;
            using (var stream = new StreamReader(file.InputStream))
            {
                fileBytes = stream.ReadToEnd().ToArray();
            }
            var data = Encoding.ASCII.GetBytes(fileBytes);

            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception exception)
            {
                throw exception;
            }


            //var postData = "title=" + post.title +
            //                   "&content=" + post.content + // Inclue HTML Markup
            //                   "&status=publish" +
            //                   "&date=" + TimeZone.CurrentTimeZone.ToUniversalTime(post.date).ToString("yyyy-MM-ddTHH:mm:ss");

            //set in Request option
            //var data = Encoding.UTF8.GetBytes(postData);
            //request.ContentLength = data.Length;
            return "";
        }
        
        public  async Task<string> templateUpload(Stream file , string FileName)
        {
            //create web request 
            var webRequest = (HttpWebRequest)WebRequest.Create(GetApiUrl("media")); 
            webRequest.Headers["Authorization"] = GetTokenBasic(); // set token in api
            var fileName = "attachment; filename="+FileName ; //fileName for Disposition header 
            webRequest.Headers.Add("Content-Disposition", fileName); 
            webRequest.ContentType = "multipart/form-data"; 
            webRequest.Method = "POST";

            /***
             * new method for byte array file
             ***/
            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            byte[] bytes = ms.ToArray(); 
            
            //using (Stream stream = file.InputStream) //reading stream file
            //{
            //    MemoryStream ms = new MemoryStream(); 
            //    stream.CopyTo(ms); 
            //    bytes = ms.ToArray(); //convert stream to byte[]
            //    ms.Close();
            //}

            //byte array technical
            #region oldByteArray
            //char[] data;
            //using (var stream = new StreamReader(image.InputStream))
            //{
            //    data = stream.ReadToEnd().ToArray();
            //}
            //byte[] bytes = Encoding.ASCII.GetBytes(data);
            #endregion

            try
            { // write stream in request
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            JToken id; 
            try
            { // send request to api
                var webResponse = await webRequest.GetResponseAsync();
                
                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                var read = sr.ReadToEnd().Trim();
                var json = JObject.Parse(read);
                id = json["id"];
            }
            catch (WebException ex)
            {
                throw ex;
            }
            return id.ToString();
        }







        public static featurImage GetFeatureImageIdByCategoryID(categoryPosts categryName)
        {
            switch (categryName)
            {
                case categoryPosts.learning:
                    return featurImage.learning;
                case categoryPosts.tips:
                    return featurImage.tip;
                case categoryPosts.trick:
                    return featurImage.trick;
                case categoryPosts.intro:
                    return featurImage.intro;
                case categoryPosts.programming:
                    return featurImage.programming;
                case categoryPosts.marketing:
                    return featurImage.marketing;
                case categoryPosts.js:
                    return featurImage.js;
                default:
                    return 0;
            }
        }
    }

    //for category id
    public enum categoryPosts
    {
        learning,
        tips,
        trick,
        intro,
        programming,
        marketing,
        js
    }

    //for tag id 
    public enum TagPosts
    {

    }

    //for feature image id for category
    public enum featurImage
    {
        js = 4946,
        csharp = 40,
        bootstrap,
        jquery,
        trick,
        learning,
        tip,
        intro ,
        programming ,
        marketing
    }
}
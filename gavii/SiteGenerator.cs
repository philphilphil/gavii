using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gavii
{
    class SiteGenerator
    {

        //generate 

        public void GenerateWebsite()
        {
            var layoutUrl = "federundblatt/Layout/";
            var outputUrl = "federundblatt/Output/";
            var postsFolder = "federundblatt/posts";

            var layoutHtml = File.ReadAllText(layoutUrl + "_Layout.html");
            var pageLayoutHtml = File.ReadAllText(layoutUrl + "_Page.html");
            var postLayoutHtml = File.ReadAllText(layoutUrl + "_Post.html");
            var galleryLayoutHtml = File.ReadAllText(layoutUrl + "_Gallery.html");
            var css = File.ReadAllText(layoutUrl + "style.css");

            //generate index page with gallery
            var indexPage = layoutHtml.Replace("{{content}}", galleryLayoutHtml);
            using (FileStream fs = File.Create(outputUrl + "Index.html"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(indexPage);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            //css
            using (FileStream fs = File.Create(outputUrl + "style.css"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(css);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            //generate posts

            var posts = Directory.GetDirectories(postsFolder);
            foreach (var post in posts)
            {
                var postfile = File.ReadAllText(post + "/post.html");
                
            }
        }
    }
}
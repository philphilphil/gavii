﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gavii
{
    class SiteGenerator
    {

        private List<Page> Pages { get; set; }
        private List<Post> Posts { get; set; }

        private readonly string layoutUrl = "federundblatt/Layout/";
        private readonly string outputUrl = "federundblatt/Output/";
        private readonly string postsFolder = "federundblatt/posts";
        private readonly string pagesFolder = "federundblatt/pages";

        public void GenerateWebsite()
        {
            var layoutHtml = File.ReadAllText(layoutUrl + "_Layout.html");
            var pageLayoutHtml = File.ReadAllText(layoutUrl + "_Page.html");
            var postLayoutHtml = File.ReadAllText(layoutUrl + "_Post.html");
            var galleryLayoutHtml = File.ReadAllText(layoutUrl + "_Gallery.html");
            var css = File.ReadAllText(layoutUrl + "style.css");

            //gather all info needed
            GetPages();
            GetPosts();

            //todo: Fill layout with links for pages. For now hardcoded in _Layout.html

            //index page with gallery
            var indexPage = layoutHtml.Replace("{{content}}", galleryLayoutHtml).Replace("{{cssForwarder}}", "");
            WriteFile(indexPage, outputUrl + "index.html");

            //css
            WriteFile(css, outputUrl + "style.css");

            //pages
            foreach (var p in this.Pages)
            {
                var pageLayout = pageLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                var completePage = layoutHtml.Replace("{{content}}", pageLayout).Replace("{{cssForwarder}}", "../../");
                string folderPath = outputUrl + p.Name;

                Directory.CreateDirectory(folderPath);

                WriteFile(completePage, folderPath + "/index.html");
            }

            //posts
            foreach (Post p in this.Posts)
            {
                var postLayout = postLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                string folderPath = outputUrl + "/posts/" + p.Name;

                //images
                if (p.GalleryImage != null)
                {
                    string imgHtml = @"<img src='{{path}}' alt='" + p.Name + "'><br />\r\n";
                    string allImages = "";
                    foreach (var i in p.Images)
                    {
                        allImages += imgHtml.Replace("{{path}}", i.Name);
                    }

                    postLayout = postLayout.Replace("{{Images}}", allImages);

                    p.GalleryImage.CopyTo(folderPath + "/" + p.GalleryImage.Name, true);
                    p.Images.ForEach(i => i.CopyTo(folderPath + "/" + i.Name, true));

                    postLayout = postLayout.Replace("{{GalleryImage}}", "<img src='"+ p.GalleryImage.Name + "' alt='" + p.Name + "'><br />\r\n");

                }

                var completePost = layoutHtml.Replace("{{content}}", postLayout).Replace("{{cssForwarder}}", "../../");


                Directory.CreateDirectory(folderPath);
                WriteFile(completePost, folderPath + "/index.html");
            }
        }

        private void WriteFile(string content, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(content);
                fs.Write(info, 0, info.Length);
            }
        }

        private void GetPages()
        {
            this.Pages = new List<Page>();
            var pages = Directory.GetFiles(pagesFolder);
            foreach (string page in pages)
            {
                this.Pages.Add(new Page(page));
            }
        }

        private void GetPosts()
        {
            this.Posts = new List<Post>();
            var posts = Directory.GetDirectories(postsFolder);
            foreach (string post in posts)
            {
                this.Posts.Add(new Post(post));
            }
        }
    }
}
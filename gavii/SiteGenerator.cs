using System;
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

            //generate index page with gallery
            var indexPage = layoutHtml.Replace("{{content}}", galleryLayoutHtml).Replace("{{cssForwarder}}", "");
            using (FileStream fs = File.Create(outputUrl + "index.html"))
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
            foreach (Post p in this.Posts)
            {
                var pageLayout = pageLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                var completePage = layoutHtml.Replace("{{content}}", pageLayout).Replace("{{cssForwarder}}", "../../");

                Directory.CreateDirectory(outputUrl + "/posts/" + p.Name);

                using (FileStream fs = File.Create(outputUrl + "/posts/" + p.Name + "/index.html"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(completePage);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
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
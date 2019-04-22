using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
            //base setup
            Directory.CreateDirectory(outputUrl + "/thumbnails/");

            var layoutHtml = File.ReadAllText(layoutUrl + "_Layout.html");
            var pageLayoutHtml = File.ReadAllText(layoutUrl + "_Page.html");
            var postLayoutHtml = File.ReadAllText(layoutUrl + "_Post.html");
            var galleryLayoutHtml = File.ReadAllText(layoutUrl + "_Gallery.html");
            var css = File.ReadAllText(layoutUrl + "style.css");

            //gather all info needed
            GetPages();
            GetPosts();

            //todo: Fill layout with links for posts. For now hardcoded in _Layout.html

            //todo: put html into _Gallery.html and add some kind of templating lang
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='col-sm-6 col-md-4'>");
            sb.Append("<a class='lightbox' href='{{PostUrl}}'>");
            sb.Append("<img src='thumbnails/{{PostThumb}}' alt='Park'>");
            sb.Append("<div class='overlay'>");
            sb.Append("<div class='text'>{{Name}}</div></div></a></div>");

            //generate thumbnails for gallery
            string gallery = "";
            foreach (Post p in this.Posts)
            {
                if (p.GalleryImage == null)
                    continue;

                //todo: resize
                using (Image<Rgba32> image = Image.Load(p.GalleryImage.FullName))
                {
                    image.Mutate(ctx => ctx.Resize(image.Width / 2, image.Height / 2));
                    image.Save(outputUrl + "/thumbnails/" + p.Name + p.GalleryImage.Extension);
                }

                gallery += sb.Replace("{{PostThumb}}", p.Name + p.GalleryImage.Extension).Replace("{{Name}}", p.Name).Replace("{{PostUrl}}", "/posts/" + p.Name);
            }

            var galleryHtml = galleryLayoutHtml.Replace("{{Images}}", gallery);

            //index page with gallery
            var indexPage = layoutHtml.Replace("{{content}}", galleryHtml).Replace("{{cssForwarder}}", "");
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
                Directory.CreateDirectory(folderPath);

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

                    postLayout = postLayout.Replace("{{GalleryImage}}", "<img src='" + p.GalleryImage.Name + "' alt='" + p.Name + "'><br />\r\n");

                }

                var completePost = layoutHtml.Replace("{{content}}", postLayout).Replace("{{cssForwarder}}", "../../");

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
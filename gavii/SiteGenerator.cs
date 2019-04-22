using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            //Base setup
            Directory.CreateDirectory(outputUrl + "/thumbnails/");
            var layoutHtml = File.ReadAllText(layoutUrl + "_Layout.html");
            var pageLayoutHtml = File.ReadAllText(layoutUrl + "_Page.html");
            var postLayoutHtml = File.ReadAllText(layoutUrl + "_Post.html");
            var galleryLayoutHtml = File.ReadAllText(layoutUrl + "_Gallery.html");
            var css = File.ReadAllText(layoutUrl + "style.css");

            //gather all info needed from setup folders
            GetPosts();
            GetPages();

            //todo: Fill layout with links for posts. For now hardcoded in _Layout.html

            //generate thumbnails and gallery
            string gallery = GenerateGalleryHtml();
            var galleryHtml = galleryLayoutHtml.Replace("{{Images}}", gallery);

            //Write index page
            var indexPage = layoutHtml.Replace("{{content}}", galleryHtml).Replace("{{cssForwarder}}", "").Replace("{{Title}}", "");
            WriteFile(indexPage, outputUrl + "index.html");

            //Write css page
            WriteFile(css, outputUrl + "style.css");

            GeneratePages(layoutHtml, pageLayoutHtml);

            GeneratePosts(layoutHtml, postLayoutHtml);
        }

        private void GeneratePosts(string layoutHtml, string postLayoutHtml)
        {
            foreach (Post p in this.Posts)
            {
                var postLayout = postLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                string folderPath = outputUrl + "/posts/" + p.UrlName;
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

                    postLayout = postLayout.Replace("{{GalleryImage}}", "<img src='" + p.GalleryImage.Name + "' alt='" + p.UrlName + "'><br />\r\n");

                }

                var completePost = layoutHtml.Replace("{{content}}", postLayout).Replace("{{cssForwarder}}", "../../").Replace("{{Title}}", " - " + p.Name);

                WriteFile(completePost, folderPath + "/index.html");
            }
        }

        private void GeneratePages(string layoutHtml, string pageLayoutHtml)
        {
            foreach (var p in this.Pages)
            {
                var pageLayout = pageLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                var completePage = layoutHtml.Replace("{{content}}", pageLayout).Replace("{{cssForwarder}}", "../../").Replace("{{Title}}", " - " + p.Name);
                string folderPath = outputUrl + p.Name;

                Directory.CreateDirectory(folderPath);

                WriteFile(completePage, folderPath + "/index.html");
            }
        }

        private string GenerateGalleryHtml()
        {
            string gallery = "";
            foreach (Post p in this.Posts)
            {
                if (p.GalleryImage == null)
                    continue;

                //todo: resize
                using (Image<Rgba32> image = Image.Load(p.GalleryImage.FullName))
                {
                    ResizeOptions ro = new ResizeOptions();
                    ro.Size = new SixLabors.Primitives.Size(600, 600);
                    ro.Mode = ResizeMode.Max;

                    image.Mutate(ctx => ctx.Resize(ro));
                    image.Save(outputUrl + "/thumbnails/" + p.UrlName + p.GalleryImage.Extension);
                }
                gallery += GetGalleryImageHtmlString(p);
              
            }

            return gallery;
        }

        private string GetGalleryImageHtmlString(Post p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='col-xs-6 col-sm-4'>");
            sb.Append("<a class='lightbox' href='{{PostUrl}}'>");
            sb.Append("<img src='thumbnails/{{PostThumb}}' alt='Park'>");
            sb.Append("<div class='overlay'>");
            sb.Append("<div class='text'>{{Name}}</div></div></a></div>");


            return sb.Replace("{{PostThumb}}", p.UrlName + p.GalleryImage.Extension).Replace("{{Name}}", p.Name).Replace("{{PostUrl}}", "/posts/" + p.UrlName).ToString();
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
                this.Pages.Add(new Page(page, this.Posts.Select(x => x.Name).ToList()));
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

            this.Posts = this.Posts.ToList().OrderByDescending(x => x.Date).ToList();
        }
    }
}
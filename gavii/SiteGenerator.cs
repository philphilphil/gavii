using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gavii
{
    class SiteGenerator
    {

        private List<Page> Pages { get; set; }
        private List<Post> Posts { get; set; }

        public string LayoutUrl { get; set; }

        public string OutputUrl { get; set; }

        public string PostsFolder { get; set; }

        public string PagesFolder { get; set; }

        private string SiteName { get; set; }



        public void GenerateWebsite()
        {
            //SiteName = GetSiteName();

            LayoutUrl = "Layout/";
            OutputUrl = "Output/";
            PostsFolder = "posts";
            PagesFolder = "pages";

            //Base setup
            Directory.CreateDirectory(OutputUrl + "/thumbnails/");
            var layoutHtml = File.ReadAllText(LayoutUrl + "_Layout.html");
            var pageLayoutHtml = File.ReadAllText(LayoutUrl + "_Page.html");
            var postLayoutHtml = File.ReadAllText(LayoutUrl + "_Post.html");
            var galleryLayoutHtml = File.ReadAllText(LayoutUrl + "_Gallery.html");
            var css = File.ReadAllText(LayoutUrl + "style.css");

            //gather all info needed from setup folders
            GetPosts();
            GetPages();

            //todo: Fill layout with links for posts. For now hardcoded in _Layout.html

            //generate thumbnails and gallery
            string gallery = GenerateGalleryHtml();
            var galleryHtml = galleryLayoutHtml.Replace("{{Images}}", gallery);

            //Write index page
            var indexPage = layoutHtml.Replace("{{content}}", galleryHtml).Replace("{{cssForwarder}}", "").Replace("{{Title}}", "");
            WriteFile(indexPage, OutputUrl + "index.html");

            //Write css page
            WriteFile(css, OutputUrl + "style.css");

            GeneratePages(layoutHtml, pageLayoutHtml);

            GeneratePosts(layoutHtml, postLayoutHtml);
        }

        private string GetSiteName()
        {
            string gaviiConfigFile = ".gaviiSite";
            if (!File.Exists(gaviiConfigFile))
            {
                throw new Exception("Not in a gavii site filder");
            }
            var content = File.ReadAllText(gaviiConfigFile);
            var match = Regex.Match(content, @"(Name:)(.*)");

            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[2].Value;
            }
            else
            {
                throw new Exception("Error reading gaviiSite-Config.");
            }
        }

        private void GeneratePosts(string layoutHtml, string postLayoutHtml)
        {
            foreach (Post p in this.Posts)
            {
                var postLayout = postLayoutHtml.Replace("{{Text}}", p.Text).Replace("{{Name}}", p.Name);
                string folderPath = OutputUrl + "/posts/" + p.UrlName;
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
                string folderPath = OutputUrl + p.Name;

                Directory.CreateDirectory(folderPath);

                WriteFile(completePage, folderPath + "/index.html");
            }
        }

        private string GenerateGalleryHtml()
        {
            string gallery = "<div class='row'>";
            int i = 0;
            foreach (Post p in this.Posts)
            {
                if (p.GalleryImage == null)
                    continue;

                i++;

                //resize
                using (Image<Rgba32> image = Image.Load(p.GalleryImage.FullName))
                {
                    ResizeOptions ro = new ResizeOptions();
                    ro.Size = new SixLabors.Primitives.Size(600, 600);
                    ro.Mode = ResizeMode.Max;

                    image.Mutate(ctx => ctx.Resize(ro));
                    image.Save(OutputUrl + "/thumbnails/" + p.UrlName + p.GalleryImage.Extension);
                }

                gallery += GetGalleryImageHtmlString(p);

                if (i == 3)
                {
                    //close row add new one
                    gallery += "</div><div class='row'>";
                    i = 0;
                }
            }

            if (this.Posts.Count % 3 > 0)
            {
                //add empty posts to make all tiles the same size
                int postAmount = 3 - (this.Posts.Count % 3);

                for (int a = 0; a < postAmount; a++)
                {
                    gallery += "<div class='col-sm'></div>";
                }
            }

            gallery += "</div>";//end row

            return gallery;
        }

        private string GetGalleryImageHtmlString(Post p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='col-sm'>");
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
            var pages = Directory.GetFiles(PagesFolder);
            foreach (string page in pages)
            {
                this.Pages.Add(new Page(page, this.Posts.Select(x => x.Name).ToList()));
            }
        }

        private void GetPosts()
        {
            this.Posts = new List<Post>();
            var posts = Directory.GetDirectories(PostsFolder);
            foreach (string post in posts)
            {
                if (post.Replace(PostsFolder, "").StartsWith("\\-"))
                    continue;

                this.Posts.Add(new Post(post));
            }

            this.Posts = this.Posts.ToList().OrderByDescending(x => x.Date).ToList();
        }
    }
}
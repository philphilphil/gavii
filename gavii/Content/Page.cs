using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gavii
{
    internal class Page : ContentReader
    {
        public string Name { get; set; }
        public string Text { get; set; }

        private List<Post> Posts { get; set; }

        public Page(string page, List<Post> posts)
        {
            var postText = File.ReadAllLines(page);
            this.Name = GetFileContentWithRegex(@"(Name:)(.*)", postText[0]).Trim();
            this.Text = GetText(2, postText);
            this.Posts = posts.OrderBy(s => s.Name).ToList();

            CheckForSpecialTextHandling();
        }

        //Implement special things here.
        private void CheckForSpecialTextHandling()
        {
            if (this.Text.StartsWith("{{ListAllPosts"))
            {
                this.Text = "";
                this.Text += "<ul>";
                //todo: order by date or name etc
                foreach (var p in Posts)
                {
                    this.Text += @"<li><a href='/posts/" + p.UrlName + "'>" + p.Name + "</a><br /></li>";
                }
                this.Text += "</ul>";
            }
        }
    }
}
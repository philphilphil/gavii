using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace gavii
{
    class Post : ContentReader
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string GPS { get; set; }
        public List<string> Tags { get; set; }
        public string Text { get; set; }
        public Post(string post)
        {
            var postText = File.ReadAllLines(post + "/post.html");
            this.Name = GetFileContentWithRegex(@"(Name:)(.*)", postText[0]).Trim();
            this.Date = GetDate(postText[1].Trim());
            this.Tags = GetTags(postText[2].Trim());
            this.Text = GetText(5, postText);
        }


        private List<string> GetTags(string v)
        {
            //todo
            List<string> tags = new List<string>();
            tags.Add("Test");

            return tags;
        }

        private DateTime GetDate(string v)
        {

            if (v.Contains("{now}"))
            {
                return DateTime.Now;
            }

            return DateTime.Parse(v);
        }


    }
}

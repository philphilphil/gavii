using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace gavii
{
    class Post
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
            this.Text = GetText(postText);
        }

        private string GetText(string[] postText)
        {
            var text = "";

            for (int i = 5; i < postText.Length; i++)
            {
                text += postText[i];
            }
            return text;
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

        private string GetFileContentWithRegex(string pattern, string text)
        {
            RegexOptions options = RegexOptions.Multiline;

            foreach (Match m in Regex.Matches(text, pattern, options))
            {
                return m.Groups[2].Value;
            }

            return "";
        }
    }
}

using System.IO;

namespace gavii
{
    internal class Page : ContentReader
    {
        public string Name { get; set; }
        public string Text { get; set; }

        public Page(string page)
        {
            var postText = File.ReadAllLines(page);
            this.Name = GetFileContentWithRegex(@"(Name:)(.*)", postText[0]).Trim();
            this.Text = GetText(2, postText);
        }
    }
}
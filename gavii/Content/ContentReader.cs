using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace gavii
{
    class ContentReader
    {
        public string GetText(int startRow, string[] postText)
        {
            var text = "";

            for (int i = startRow; i < postText.Length; i++)
            {
                text += postText[i];
            }
            return text;
        }

        public string GetFileContentWithRegex(string pattern, string text)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace gavii
{
    class SiteManager
    {

        public void AddNewSite(string Name)
        {
            string executingPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Name);
            string exampleSitePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ExampleSite");

            foreach (string dirPath in Directory.GetDirectories(exampleSitePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(exampleSitePath, executingPath));
            }

            foreach (string newPath in Directory.GetFiles(exampleSitePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(exampleSitePath, executingPath), true);
            }
        }
    }
}

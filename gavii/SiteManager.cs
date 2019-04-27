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
            var executingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Directory.CreateDirectory(executingPath + "\\" + Name);
        }

        private void CheckIfInSiteDirectory()
        {
            //  throw new Exception("You are not in the directory of a site.");
        }
    }
}

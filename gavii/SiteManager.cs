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

            //create directories
            Directory.CreateDirectory(executingPath + "\\" + Name);
            Directory.CreateDirectory(executingPath + "\\" + Name + "\\Layout");
            Directory.CreateDirectory(executingPath + "\\" + Name + "\\pages");
            Directory.CreateDirectory(executingPath + "\\" + Name + "\\posts\\1-Eichhoernchen");
            Directory.CreateDirectory(executingPath + "\\" + Name + "\\posts\\-EmptyPost");
        }

        private void CheckIfInSiteDirectory()
        {
            //  throw new Exception("You are not in the directory of a site.");
        }
    }
}

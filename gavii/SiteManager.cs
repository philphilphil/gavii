﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace gavii
{
    class SiteManager
    {

        public void AddNewSite(string Name)
        {

            Directory.CreateDirectory("Posts/test");
        }

        private void CheckIfInSiteDirectory()
        {
          //  throw new Exception("You are not in the directory of a site.");
        }
    }
}

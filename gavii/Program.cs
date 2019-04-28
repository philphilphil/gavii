using System;

namespace gavii
{
    class Program
    {
        static void Main(string[] args)
        {
            // todo: use some fancy lib or w/ e
            if (args.Length == 0)
            {
                //no command = generate output
                SiteGenerator sg = new SiteGenerator();
                sg.GenerateWebsite();
            }
            else if (args[0] == "new")
            {
                if (args.Length > 1 && args[1] != null)
                {
                    SiteManager sm = new SiteManager();
                    sm.AddNewSite(args[1]);
                }
            }
            else
            {
                Console.WriteLine("Invalid command.");
            }

            return;
        }
    }
}

using System;

namespace gavii
{
    class Program
    {
        static void Main(string[] args)
        {


            SiteGenerator sg = new SiteGenerator();
            sg.GenerateWebsite();

            return;

            //SiteManager sm = new SiteManager();
            //sm.AddNewPage("Test");



            //todo: use some fancy lib or w/e
            if (args.Length == 0)
            {
                Console.WriteLine("No command provided.");
            }
            else if (args[0] == "add")
            {
                if (args[1] == "page")
                {
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Provide a name for the page.");
                        return;
                    }

                    Console.WriteLine(args[2]);

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

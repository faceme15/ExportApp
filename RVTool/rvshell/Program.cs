using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace rvshell
{

    /**
     * Performs full export from RVTools
     */
    class Program
    {

        static void Main(string[] args)
        {

            var logic = new Logic();
            var rvtools = new RVTools();

            // Get the user's credentials
            var credentials = PresentationLogic.GetCredentials();

            foreach (var server in logic.servers)
            {
                Console.WriteLine($"Querying {server} for ESXi metrics and features.");
                rvtools.Query(server, credentials.Item1, credentials.Item2, "1");
            }

            Console.Write("Press the Enter key once RVTools has completed export to complete processing ");
            Console.ReadLine();

        }
    }

    class PresentationLogic
    {
        
        public static Tuple<string, string> GetCredentials() {

            Console.Write("Please enter your admin username: ");
            var username = Console.ReadLine();

            Console.Write("Please enter your admin password: ");
            var password = Console.ReadLine();

            return new Tuple<string, string>(username, password);

        }

    }

    class Logic
    {

        public string basePath = @"C:\Testing";

        public string[] servers = new[] { "ssdsbrvc0001", "ssisusvc0005", "usssusvc001", "usssusvc010" };

        public DirectoryInfo CreateServerDirectory(string path, string server)
        {

            var date = DateTime.Now;
            var dateString = $"{date.Month}/{date.Day}/{date.Year}";

            var directory = $"{basePath}/{server}/{dateString}";

            return Directory.CreateDirectory(directory);

        }

    }

}

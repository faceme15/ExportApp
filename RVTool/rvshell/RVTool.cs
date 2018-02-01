using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rvshell
{
    public class RVTools
    {

        public const string EXECUTABLE = "RVTools.exe";

        public string TEMP_DIR = $"{System.Environment.CurrentDirectory}\\temp\\rvtools";

        public string homePath = @"C:\Program Files (x86)\RobWare\RVTools";

        public RVTools()
        {

            if (!Directory.Exists(TEMP_DIR))
                Directory.CreateDirectory(TEMP_DIR);

        }

        public string Query(string server, string username, string password, string sessionId)
        {
            DateTime currentDate = DateTime.Now;

            string exportPath = $"{TEMP_DIR}\\{currentDate.Year}-{currentDate.Month}-{currentDate.Day}\\{sessionId}\\{server}";

            try
            {

                using (var process = GenerateCsvExportProcess(server, username, password, exportPath))
                {
                    process.Start();
                }

            }
            finally
            {

            }

            return exportPath;

        }

        private Process GenerateCsvExportProcess(string server, string username, string password, string exportPath)
        {
            var process = new Process();
            var processStartInfo = new ProcessStartInfo();
            
            if (!Directory.Exists(exportPath))
                Directory.CreateDirectory(exportPath);

            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.FileName = $"{homePath}\\{EXECUTABLE}";
            processStartInfo.Arguments = $"\\C -s {server} -u {username} -p {password} -c ExportAll2csv -d \"{exportPath}\"";
            process.StartInfo = processStartInfo;

            return process;

        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using rvshell;

namespace rvvisual
{
    public partial class vCenter : Form
    {

        private RVTools rvtools = new RVTools();

        private string[] servers = new[] { "ssdsbrvc0001", "ssisusvc0005", "usssusvc001", "usssusvc010" };

        public vCenter()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        /**
         * Performs a query using RV Tools
         */
        private void button1_Click(object sender, EventArgs e)
        {
            bool resetState = true;
            string username = this.textBox1.Text.Trim();
            string password = this.textBox2.Text;
            string targetDirectory = this.textBox4.Text.Trim();

            List<String> directoriesToMerge = new List<string>();
            var randomStr = Path.GetRandomFileName().Replace(".", "");
            var uuid = randomStr.Substring(0, randomStr.Length / 2);

            if (username == null || username == "") {
                MessageBox.Show("Username is not set");
                return;
            }
            if (password == null || password == "")
            {
                MessageBox.Show("Password is not set");
                return;
            }
            if (targetDirectory == null || targetDirectory == "")
            {
                MessageBox.Show("Target Directory is not set");
                return;
            }

            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            try
            {

                foreach (string server in servers)
                {
                    var folder = rvtools.Query($"{server}.nasa.group.atlascopco.com", username, password, uuid);

                    directoriesToMerge.Add(folder);
                }

                DialogResult result = MessageBox.Show("Press Yes once all of the queries have completed to merge CSVs.", "Waiting for Queries", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {

                    var csvFileInfos = new List<FileInfo>();

                    // Get information about the CSV files for grouping
                    foreach (string toMerge in directoriesToMerge)
                        foreach (string csvFilePath in Directory.GetFiles(toMerge, "*.csv", SearchOption.AllDirectories))
                            csvFileInfos.Add(new FileInfo(csvFilePath));

                    var groupedCSVs = csvFileInfos.GroupBy(f => f.Name);

                    foreach (var groupedCSVWrapper in groupedCSVs)
                    {
                        string filename = groupedCSVWrapper.Count() > 0 ? groupedCSVWrapper.First().Name : null;
                        string header = null;
                        var mergedCsv = new List<string>();

                        if (filename == null) continue;

                        foreach (var csv in groupedCSVWrapper.ToList())
                        {
                            string[] lines = File.ReadAllLines(csv.FullName);

                            if (header == null && lines.Length > 0)
                                header = lines.First();

                            for (var i = 1; i < lines.Length; i++)
                                mergedCsv.Add(lines[i]);

                        }

                        var mergedFile = File.CreateText(Path.Combine(targetDirectory, filename));

                        mergedFile.WriteLine(header);

                        foreach (var line in mergedCsv)
                            mergedFile.WriteLine(line);

                    }

                    MessageBox.Show("Done merging CSVs!", "Yerp!");
                           
                }
                else if (result == DialogResult.No)
                {
                    resetState = false;
                }
                else
                {
                    MessageBox.Show("Aborting CSV Merge");
                    System.Environment.Exit(-1);
                }


            }
            catch(Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Exception!");
                System.Environment.Exit(1);
            }
            finally {
                if (resetState)
                {
                    this.textBox1.Text = "";
                    this.textBox2.Text = "";
                    this.textBox4.Text = "";
                }
            }
        }

        /**
         * Open the Folder Browser Dialog for Destination Target
         */
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialog = this.folderBrowserDialog1.ShowDialog();

                if (dialog == DialogResult.OK)
                    this.textBox4.Text = this.folderBrowserDialog1.SelectedPath;

            }
            finally
            {
                this.folderBrowserDialog1.SelectedPath = "";
            }
        }
    }
}

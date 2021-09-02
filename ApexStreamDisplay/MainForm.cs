using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace ApexStreamDisplay
{
    public partial class MainForm : Form
    {
        public static string _version = "v2.1";
        public static string _directory = Directory.GetCurrentDirectory();
        public StreamWriter _logger;
        public static int _kills;
        public static int _wins;
        public static int _rp;
        public static int _games;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Stream Display ";

            if (!Directory.Exists(_directory + @"\TextFiles"))
            {
                Directory.CreateDirectory(_directory + @"\TextFiles");
            }
            else if (!Directory.Exists(_directory + @"\Data"))
            {
                Directory.CreateDirectory(_directory + @"\Data");
            }
            else if (!Directory.Exists(_directory + @"\Logs"))
            {
                Directory.CreateDirectory(_directory + @"\Logs");
                _logger = new StreamWriter(_directory + @"\Logs\latest.log");
            }
            else
            {
                _logger = new StreamWriter(_directory + @"\Logs\latest.log");
            }
        }

        public void WriteKillsToFile(string value)
        {
            liveKillsText.Text = value;
            using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\Kills.txt"))
            {
                sw.WriteLine(value);
            }
        }

        public void WriteWinsToFile(string value)
        {
            liveWinsText.Text = value;
            using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\Wins.txt"))
            {
                sw.WriteLine(value);
            }
        }

        public void WriteRPToFile(int value)
        {
            string format = outputFormatText.Text;

            if (value >= 0)
            {
                string valueString = "+" + value.ToString();
                format = format.Replace("$rp", valueString);

                liveRPText.Text = format;
                using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\RP.txt"))
                {
                    sw.WriteLine(format);
                }
            }
            else if (value < 0)
            {
                string valueString = value.ToString();
                format = format.Replace("$rp", valueString);

                liveRPText.Text = format;
                using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\RP.txt"))
                {
                    sw.WriteLine(format);
                }
            }
        }

        public void LoadFromFiles()
        {
            StreamReader srKills = new StreamReader(_directory + @"\TextFiles\Kills.txt");
            StreamReader srWins = new StreamReader(_directory + @"\TextFiles\Wins.txt");
            StreamReader srRP = new StreamReader(_directory + @"\TextFiles\RP.txt");

            _kills = Convert.ToInt32(srKills.ReadLine());
            _wins = Convert.ToInt32(srWins.ReadLine());
            _rp = Convert.ToInt32(srRP.ReadLine());

            srKills.Close();
            srWins.Close();
            srRP.Close();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Kills
                _kills = _kills + Convert.ToInt32(killsTextBox.Text);
                WriteKillsToFile(_kills.ToString());


                // Wins
                if (wonCheckBox.Checked == true)
                {
                    _wins = _wins + 1;
                    WriteWinsToFile(_wins.ToString());
                }

                killsTextBox.Value = 0;
                wonCheckBox.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log(ex.Message);
            }
        }

        private void updateRPButton_Click(object sender, EventArgs e)
        {
            // RP
            try
            {
                int rp = Convert.ToInt32(rpCalcTB.Text);
                _rp = _rp + rp;

                WriteRPToFile(Convert.ToInt32(_rp));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log(ex.Message);
            }
        }

        private void killstxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader srKills = new StreamReader(_directory + @"\TextFiles\Kills.txt");
                _kills = Convert.ToInt32(srKills.ReadLine());
                srKills.Close();
                WriteKillsToFile(_kills.ToString());
            }
            catch (Exception ex)
            {
                File.Create(_directory + @"\TextFiles\Kills.txt");
                Log(ex.Message);
            }
        }

        private void winstxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader srWins = new StreamReader(_directory + @"\TextFiles\Wins.txt");
                _wins = Convert.ToInt32(srWins.ReadLine());
                srWins.Close();
                WriteWinsToFile(_wins.ToString());
            }
            catch (Exception ex)
            {
                File.Create(_directory + @"\TextFiles\Wins.txt");
                Log(ex.Message);
            }
        }

        private void rPtxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader srRP = new StreamReader(_directory + @"\TextFiles\RP.txt");
                _rp = Convert.ToInt32(srRP.ReadLine());
                srRP.Close();
                WriteRPToFile(_rp);
            }
            catch (Exception ex)
            {
                File.Create(_directory + @"\TextFiles\RP.txt");
                Log(ex.Message);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            _kills = 0;
            _wins = 0;
            _rp = 0;

            WriteKillsToFile("0");
            WriteWinsToFile("0");
            WriteRPToFile(0);
        }

        private void showFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(_directory);
        }

        private void showCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(showCheckBox.Checked == true)
            {
                apiKeyTextBox.UseSystemPasswordChar = false;
            }
            else
            {
                apiKeyTextBox.UseSystemPasswordChar = true;
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://apexlegendsapi.com/documentation.php");
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start($"https://apexlegendsapi.com/dashboard.php?key={ apiKeyTextBox.Text}");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                int playerRP = GetPlayerRP("https://api.mozambiquehe.re/bridge?version=5&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
                autoRPUpDown.Value = playerRP;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log(ex.Message);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int playerRP = GetPlayerRP("https://api.mozambiquehe.re/bridge?version=5&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);

            var rp = playerRP - autoRPUpDown.Value;

            _rp = Convert.ToInt32(rp);
            WriteRPToFile(_rp);
        }

        public int GetPlayerRP(string url)
        {
            try
            {
                var startTimestamp = DateTime.Now;

                var request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(dataStream);
                string responseString = sr.ReadToEnd();
                dynamic playerData = JsonConvert.DeserializeObject(responseString);
                int rp = Convert.ToInt32(playerData.global.rank.rankScore);
                var endTimestamp = DateTime.Now;

                var interval = endTimestamp - startTimestamp;

                string format = $"Pulled ranked score \"{playerData.global.rank.rankScore}\" from player \"{playerData.global.name}\" in {interval.TotalMilliseconds} ms";
                LogAndDisplay(format);

                return rp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log(ex.Message);
                return 0;
            }
        }

        public void LoadAPI()
        {
            string filePath = _directory + @"\Data\save.txt";
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Username:"))
                        {
                            usernameTextBox.Text = line.Replace("Username:", "");
                        }
                        else if (line.Contains("ApiKey:"))
                        {
                            apiKeyTextBox.Text = line.Replace("ApiKey:", "");
                        }
                        else if(line.Contains("StaringRP:"))
                        {
                            autoRPUpDown.Value = Convert.ToDecimal(line.Replace("StaringRP:", ""));
                        }
                        else if (line.Contains("OutputFormat:"))
                        {
                            outputFormatText.Text = line.Replace("OutputFormat:", "");
                        }
                    }
                }
            }
        }

        public void SaveAPI()
        {
            using (StreamWriter sw = new StreamWriter(_directory + @"\Data\save.txt"))
            {
                sw.WriteLine("Username:" + usernameTextBox.Text);
                sw.WriteLine("ApiKey:" + apiKeyTextBox.Text);
                sw.WriteLine("StaringRP:" + autoRPUpDown.Value);
                sw.WriteLine("OutputFormat:" + outputFormatText.Text);
                sw.Close();
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadAPI();
            _logger.WriteLine($"Apex Stream Display {_version}");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAPI();
            //SaveLog();
        }

        private void autoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoCheckBox.Checked == true)
            {
                button12.Enabled = false;
                loopTimer.Interval = Convert.ToInt32(minutesUpDown.Value * 60000);
                loopTimer.Start();

                string format = $"Started auto refresh on interval {minutesUpDown.Value * 60} seconds";
                LogAndDisplay(format);
            }
            else
            {
                button12.Enabled = true;
                loopTimer.Stop();

                string format = $"[Stopped auto refresh";
                LogAndDisplay(format);
            }
        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {
            int playerRP = GetPlayerRP("https://api.mozambiquehe.re/bridge?version=5&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);

            var rp = playerRP - autoRPUpDown.Value;

            _rp = Convert.ToInt32(rp);
            WriteRPToFile(_rp);
        }

        public void LogAndDisplay(string log)
        {
            var timestamp = DateTime.Now;
            string format = $"[{timestamp.ToString("HH:mm:ss")}] {log}";

            outputTextBox.Text = format;
            Console.WriteLine(format);
            _logger.WriteLine(format);
            _logger.Flush();
        }

        public void Log(string log)
        {
            var timestamp = DateTime.Now;
            string format = $"[{timestamp.ToString("HH:mm:ss")}] {log}";

            Console.WriteLine(format);
            _logger.WriteLine(format);
            _logger.Flush();
        }

        public void SaveLog()
        {
            var timestamp = DateTime.Now;
            _logger.Close();
            File.Move(_directory + @"\Logs\latest.log", _directory + @"\Logs\" + timestamp.ToString("yyyy-MM-dd HH-mm-ss") + ".log");
        }

        private void showLogFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(_directory + @"\Logs");
        }

        private void viewCurrentLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(_directory + @"\Logs\latest.log");
        }

        private void saveCurrentLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLog();
        }

        private void songFormatButton_Click(object sender, EventArgs e)
        {
            outputFormatText.Text = "$rp";
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            CalculateRP();
        }

        public void CalculateRP()
        {
            decimal rpTotal = 0 - matchUpDown.Value;

            if (placeUpDown.Value > 13)
            {
                rpTotal = rpTotal + (killsUpDown.Value * 10);
            }
            else
            {
                switch (placeUpDown.Value)
                {
                    case 1:
                        rpTotal = rpTotal + 100;
                        rpTotal = rpTotal + (killsUpDown.Value * 25);
                        break;

                    case 2:
                        rpTotal = rpTotal + 60;
                        rpTotal = rpTotal + (killsUpDown.Value * 20);
                        break;

                    case 3:
                        rpTotal = rpTotal + 40;
                        rpTotal = rpTotal + (killsUpDown.Value * 20);
                        break;

                    case 4:
                        rpTotal = rpTotal + 40;
                        rpTotal = rpTotal + (killsUpDown.Value * 15);
                        break;

                    case 5:
                        rpTotal = rpTotal + 30;
                        rpTotal = rpTotal + (killsUpDown.Value * 15);
                        break;

                    case 6:
                        rpTotal = rpTotal + 30;
                        rpTotal = rpTotal + (killsUpDown.Value * 12);
                        break;

                    case 7:
                        rpTotal = rpTotal + 20;
                        rpTotal = rpTotal + (killsUpDown.Value * 12);
                        break;

                    case 8:
                        rpTotal = rpTotal + 20;
                        rpTotal = rpTotal + (killsUpDown.Value * 12);
                        break;

                    case 9:
                        rpTotal = rpTotal + 10;
                        rpTotal = rpTotal + (killsUpDown.Value * 12);
                        break;

                    case 10:
                        rpTotal = rpTotal + 10;
                        rpTotal = rpTotal + (killsUpDown.Value * 12);
                        break;

                    case 11:
                        rpTotal = rpTotal + 5;
                        rpTotal = rpTotal + (killsUpDown.Value * 10);
                        break;

                    case 12:
                        rpTotal = rpTotal + 5;
                        rpTotal = rpTotal + (killsUpDown.Value * 10);
                        break;

                    case 13:
                        rpTotal = rpTotal + 5;
                        rpTotal = rpTotal + (killsUpDown.Value * 10);
                        break;
                }
            }

            rpCalcTB.Text = rpTotal.ToString();
        }

        private void matchUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateRP();
        }

        private void killsUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateRP();
        }

        private void placeUpDown_ValueChanged(object sender, EventArgs e)
        {
            CalculateRP();
        }
    }
}

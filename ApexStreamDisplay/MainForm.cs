using Newtonsoft.Json;
using Logging;
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
        public static string _version = "3.0 (Beta)";
        public static string _directory = Directory.GetCurrentDirectory();
        public static int _apiVersion = 5;

        public static int _kills;
        public static int _wins;
        public static int _rp;
        public static int _matchRP;
        public static int _games;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Apex RP Tracker ";

            if (!Directory.Exists(_directory + @"\TextFiles"))
            {
                Directory.CreateDirectory(_directory + @"\TextFiles");
            }
            else if (!Directory.Exists(_directory + @"\Data"))
            {
                Directory.CreateDirectory(_directory + @"\Data");
            }
            else if (!Directory.Exists(_directory + @"\logs"))
            {
                Directory.CreateDirectory(_directory + @"\logs");
            }
            else if (File.Exists(Path.Combine(_directory, "logs", "latest.log")))
            {
                Logger.Delete();
            }

            Logger.LogSystemInfo();
            Logger.Separate();
            Logger.WriteLine($"{this.Text} v{_version}");
            Logger.LogModules();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadAPI();
            Logger.WriteLine("==== Startup Complete ======================");
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
                    Console.WriteLine(format);
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
                Logger.Exception(ex);
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
                Logger.Exception(ex);
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
                Logger.Exception(ex);
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
                Logger.Exception(ex);
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
                Logger.Exception(ex);
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
                var player = GetPlayerData("https://api.mozambiquehe.re/bridge?version=" + _apiVersion + "&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
                autoRPUpDown.Value = Convert.ToInt32(player.global.rank.rankScore);
                _matchRP = Convert.ToInt32(player.global.rank.rankScore);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Exception(ex);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var player = GetPlayerData("https://api.mozambiquehe.re/bridge?version=5&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
            UpdateMatchHistory(player);            
        }

        void UpdateMatchHistory(dynamic playerData)
        {
            try
            {
                var rpDiff = Convert.ToInt32(playerData.global.rank.rankScore - autoRPUpDown.Value);
                int rp = Convert.ToInt32(playerData.global.rank.rankScore) - _matchRP;

                Console.WriteLine($"RP: {rp}");
                //Console.WriteLine($"Match RP: {matchRP}");
                Console.WriteLine($"Global Match RP: {_matchRP}");
                Console.WriteLine($"Player RP: {Convert.ToInt32(playerData.global.rank.rankScore)}");

                _matchRP = Convert.ToInt32(playerData.global.rank.rankScore);

                if (rp != _rp)
                {
                    string rpString = null;
                    if (rp >= 0)
                    {
                        rpString = "+" + rp.ToString();
                    }
                    else if (rp < 0)
                    {
                        rpString = rp.ToString();
                    }

                    string[] row = { DateTime.Now.ToString("hh:mm:ss tt"), playerData.realtime.selectedLegend, rpString };
                    ListViewItem lvi = new ListViewItem(row);

                    if (rp > _rp)
                    {
                        lvi.ForeColor = Color.Green;
                    }
                    else if (rp <= _rp)
                    {
                        lvi.ForeColor = Color.Red;
                    }

                    matchHistory.Items.Add(lvi);
                }

                _rp = rpDiff;
                WriteRPToFile(_rp);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public dynamic GetPlayerData(string url)
        {
            try
            {
                Logger.WriteLine("Sending API Request:");
                Logger.WriteLine($"   Url: {url}");

                var startTimestamp = DateTime.Now;
                var request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(dataStream);
                string responseString = sr.ReadToEnd();
                dynamic playerData = JsonConvert.DeserializeObject(responseString);

                if(playerData != null)
                {
                    int rp = Convert.ToInt32(playerData.global.rank.rankScore);
                    var endTimestamp = DateTime.Now;

                    var interval = endTimestamp - startTimestamp;

                    Logger.WriteLine($"   Player: {playerData.global.name}");
                    Logger.WriteLine($"   Platform: {playerData.global.platform}");
                    Logger.WriteLine($"   RP Score: {playerData.global.rank.rankScore}");
                    Logger.WriteLine($"   Response Time: {interval.TotalMilliseconds}");
                    Logger.WriteLine($"   Version: {_apiVersion}");
                    Logger.WriteLine($"   Access Type: {playerData.mozambiquehere_internal.APIAccessType}");
                    Logger.WriteLine($"   Cluster ID: {playerData.mozambiquehere_internal.ClusterID}");
                    Logger.WriteLine($"   Cluster Srv: {playerData.mozambiquehere_internal.clusterSrv}");

                    string format = $"Pulled ranked score \"{playerData.global.rank.rankScore}\" from player \"{playerData.global.name}\" in {interval.TotalMilliseconds} ms";
                    outputTextBox.Text = format;
                    return playerData;
                }
                else
                {
                    MessageBox.Show("No response from API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.WriteLine("Request returned null");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Exception(ex);
                return null;
            }
        }

        public void LoadAPI()
        {
            string filePath = _directory + @"\Data\save.txt";
            if (File.Exists(filePath))
            {
                Logger.WriteLine("Loading API info:");
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("Username:"))
                        {
                            usernameTextBox.Text = line.Replace("Username:", "");
                            Logger.WriteLine($"   {line}");
                        }
                        else if (line.Contains("ApiKey:"))
                        {
                            apiKeyTextBox.Text = line.Replace("ApiKey:", "");
                            Logger.WriteLine($"   {line}");
                        }
                        else if(line.Contains("StaringRP:"))
                        {
                            autoRPUpDown.Value = Convert.ToDecimal(line.Replace("StaringRP:", ""));
                            _matchRP = Convert.ToInt32(line.Replace("StaringRP:", ""));
                            Logger.WriteLine($"   {line}");
                        }
                        else if (line.Contains("OutputFormat:"))
                        {
                            outputFormatText.Text = line.Replace("OutputFormat:", "");
                            Logger.WriteLine($"   {line}");
                        }
                    }
                }
            }
        }

        public void SaveAPI()
        {
            Logger.WriteLine("Saving API Info:");
            using (StreamWriter sw = new StreamWriter(_directory + @"\Data\save.txt"))
            {
                sw.WriteLine("Username:" + usernameTextBox.Text);
                Logger.WriteLine("Username:" + usernameTextBox.Text);

                sw.WriteLine("ApiKey:" + apiKeyTextBox.Text);
                Logger.WriteLine("ApiKey:" + apiKeyTextBox.Text);

                sw.WriteLine("StaringRP:" + autoRPUpDown.Value);
                Logger.WriteLine("StaringRP:" + autoRPUpDown.Value);

                sw.WriteLine("OutputFormat:" + outputFormatText.Text);
                Logger.WriteLine("OutputFormat:" + outputFormatText.Text);
                sw.Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.WriteLine("==== Shutting down ==============");
            SaveAPI();
        }

        private void autoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoCheckBox.Checked == true)
            {
                button12.Enabled = false;
                loopTimer.Interval = Convert.ToInt32(minutesUpDown.Value * 60000);
                loopTimer.Start();

                string format = $"Started auto refresh on interval {minutesUpDown.Value * 60} seconds";
                outputTextBox.Text = format;
            }
            else
            {
                button12.Enabled = true;
                loopTimer.Stop();

                string format = $"[Stopped auto refresh";
                outputTextBox.Text = format;
            }
        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {
            var player = GetPlayerData("https://api.mozambiquehe.re/bridge?version=5&platform=PC&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
            UpdateMatchHistory(player);
        }

        public void SaveLog()
        {
            var timestamp = DateTime.Now;
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

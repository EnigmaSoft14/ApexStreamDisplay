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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace ApexStreamDisplay
{
    public partial class MainForm : Form
    {
        public static string _version = "v4.0";
        public static string _directory = Directory.GetCurrentDirectory();
        public static string _roamingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static int _apiVersion = 5;

        public Icon _red = new Icon(@"Icons\apex-legends-symbol-red.ico");
        public Icon _blue = new Icon(@"Icons\apex-legends-symbol-blue.ico");
        public Icon _gray = new Icon(@"Icons\apex-legends-symbol-gray.ico");

        public UpdateForm updateForm;

        public static string[] rankedTier = { "Rookie 4", "Rookie 3", "Rookie 2", "Rookie 1", "Bronze 4", "Bronze 3", "Bronze 2", "Bronze 1", "Silver 4", "Silver 3", "Silver 2", "Silver 1", "Gold 4", "Gold 3", "Gold 2", "Gold 1", "Platinum 4", "Platinum 3", "Platinum 2", "Platinum 1", "Diamond 4", "Diamond 3", "Diamond 2", "Diamond 1", "Master", "Apex Predator" };
        public static int[] rankedRP = { 0, 250, 500, 750, 1000, 1500, 2000, 2500, 3000, 3600, 4200, 4800, 5400, 6100, 6800, 7500, 8200, 9000, 9800, 10600, 11400, 12300, 13200, 14100, 15000 };
        public static int index = 0;

        public static string _prevLegend;

        public static int _kills;
        public static int _wins;
        public static int _rp;
        public static int _rpDistance;
        public static int _matchRP;
        public static int _games;

        public static int _matchKills;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Apex Stats Tracker ";
            notifyIcon.Text = this.Text;
            modeComboBox.SelectedIndex = 0;
            platformCB.SelectedIndex = 0;

            Logger.StartUp("Apex Stats Tracker", _version);
            Console.WriteLine(Logger.CurrentDirectory);

            if (!Directory.Exists(Path.Combine(Logger.CurrentDirectory, "Data")))
            {
                Directory.CreateDirectory(Path.Combine(Logger.CurrentDirectory, "Data"));
            }

            if (!Directory.Exists(_directory + @"\TextFiles"))
            {
                Directory.CreateDirectory(_directory + @"\TextFiles");
            }
        }

        #region Methods

        async void CheckVersion()
        {
            DateTime startTimestamp = DateTime.Now;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApplication", "1"));
            string repo = "EnigmaGames14/ApexStreamDisplay";
            string contentsUrl = $"https://api.github.com/repos/{repo}/releases";
            string contentsJson = await httpClient.GetStringAsync(contentsUrl);
            dynamic contents = JsonConvert.DeserializeObject(contentsJson);

            if (_version != (string)contents[0].tag_name)
            {
                if ((updateForm == null) || (updateForm.IsDisposed))
                {
                    updateForm = new UpdateForm(contentsJson);
                }

                updateForm.Show();
            }
        }

        public void LoadAPI()
        {
            try
            {
                XmlReader reader = XmlReader.Create(Path.Combine(Logger.CurrentDirectory, "data.xml"));
                //reader.ReadStartElement("Server");

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Username"))
                    {
                        string element = reader.ReadElementContentAsString();
                        usernameTextBox.Text = element.ToString();
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Platform"))
                    {
                        string element = reader.ReadElementContentAsString();
                        platformCB.Text = element.ToString();
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("ApiKey"))
                    {
                        string element = reader.ReadElementContentAsString();
                        byte[] data = Convert.FromBase64String(element);
                        string decodedString = Encoding.UTF8.GetString(data);
                        apiKeyTextBox.Text = decodedString;
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("StaringRP"))
                    {
                        string element = reader.ReadElementContentAsString();
                        autoRPUpDown.Value = Convert.ToInt32(element);
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("StaringKills"))
                    {
                        string element = reader.ReadElementContentAsString();
                        killsUpDown.Value = Convert.ToInt32(element);
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("StaringWins"))
                    {
                        string element = reader.ReadElementContentAsString();
                        winsUpDown.Value = Convert.ToInt32(element);
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("OutputRPFormat"))
                    {
                        string element = reader.ReadElementContentAsString();
                        outputFormatText.Text = element.ToString();
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("OutputRPDisFormat"))
                    {
                        string element = reader.ReadElementContentAsString();
                        rpDisFormatText.Text = element.ToString();
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("OutputKillsFormat"))
                    {
                        string element = reader.ReadElementContentAsString();
                        killsFormatTB.Text = element.ToString();
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("OutputWinsFormat"))
                    {
                        string element = reader.ReadElementContentAsString();
                        winsFormatTB.Text = element.ToString();
                    }
                }

                reader.Close();
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void SaveAPI()
        {
            byte[] data = Encoding.UTF8.GetBytes(apiKeyTextBox.Text);
            string encodedString = Convert.ToBase64String(data);

            XmlWriter writer = XmlWriter.Create(Path.Combine(Logger.CurrentDirectory, "data.xml"));
            writer.WriteStartDocument();

            writer.WriteStartElement(this.Name);
            writer.WriteAttributeString("version", _version);
            writer.WriteStartElement("Client");
            writer.WriteElementString("Username", usernameTextBox.Text);
            writer.WriteElementString("Platform", platformCB.Text);
            writer.WriteElementString("ApiKey", encodedString);
            writer.WriteElementString("StaringRP", autoRPUpDown.Value.ToString());
            writer.WriteElementString("StaringKills", killsUpDown.Value.ToString());
            writer.WriteElementString("StaringWins", winsUpDown.Value.ToString());
            writer.WriteElementString("OutputRPFormat", outputFormatText.Text);
            writer.WriteElementString("OutputRPDisFormat", rpDisFormatText.Text);
            writer.WriteElementString("OutputKillsFormat", killsFormatTB.Text);
            writer.WriteElementString("OutputWinsFormat", winsFormatTB.Text);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        void FindDistance(int compare)
        {
            int count = 0;
            foreach (var r in rankedRP)
            {
                Console.WriteLine($"{compare} < {r}");

                Console.WriteLine($"{r}\nTier: {rankedTier[count]} | Points: {rankedRP[count]}");
                _rpDistance = r;

                Console.WriteLine("Distance: " + _rpDistance);

                index = count;

                if (compare < r)
                {
                    break;
                }

                if (rankedTier[count] == "Master")
                {
                    FindPredDistance("https://api.mozambiquehe.re/predator?auth=" + apiKeyTextBox.Text);
                }

                count++;
            }
        }

        void FindPredDistance(string url)
        {
            try
            {
                Icon start = notifyIcon.Icon;
                notifyIcon.Icon = _blue;

                Logger.WriteLine("Sending API Request:");
                Logger.WriteLine($"   Url: {url}");
                outputTextBox.Text = "Sending API Request...";

                var startTimestamp = DateTime.Now;
                var request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(dataStream);
                string responseString = sr.ReadToEnd();

                if (responseString != null)
                {
                    //Console.WriteLine(responseString);
                    dynamic data = JsonConvert.DeserializeObject(responseString);
                    var endTimestamp = DateTime.Now;

                    var interval = endTimestamp - startTimestamp;

                    Logger.WriteLine($"   Platform: {platformCB.Text}");

                    string format = null;
                    switch (platformCB.SelectedIndex)
                    {
                        case 0:
                            format = $"Pulled pred cap \"{data.RP.PC.val}\" in {interval.TotalMilliseconds} ms";
                            Logger.WriteLine($"   Pred Score: {data.RP.PC.val}");
                            _rpDistance = Convert.ToInt32(data.RP.PC.val);
                            break;

                        case 1:
                            format = $"Pulled pred cap \"{data.RP.X1.val}\" in {interval.TotalMilliseconds} ms";
                            Logger.WriteLine($"   Pred Score: {data.RP.X1.val}");
                            _rpDistance = Convert.ToInt32(data.RP.X1.val);
                            break;

                        case 2:
                            format = $"Pulled pred cap \"{data.RP.PS4.val}\" in {interval.TotalMilliseconds} ms";
                            Logger.WriteLine($"   Pred Score: {data.RP.PS4.val}");
                            _rpDistance = Convert.ToInt32(data.RP.PS4.val);
                            break;
                    }

                    Logger.WriteLine($"   Response Time: {interval.TotalMilliseconds}");
                    Logger.WriteLine($"   Version: {_apiVersion}");

                    string formatBox = rpDisFormatText.Text;
                    string valueString = _rpDistance.ToString();
                    formatBox = formatBox.Replace("$rp", valueString);
                    formatBox = formatBox.Replace("$rank", "Apex Predator");

                    distanceRPTextBox.Text = formatBox;
                    using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\RP-Distance.txt"))
                    {
                        sw.WriteLine(formatBox);
                    }
                }
                else
                {
                    MessageBox.Show("No response from API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.WriteLine("Request returned null");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        dynamic GetPlayerData(string url)
        {
            try
            {
                Icon start = notifyIcon.Icon;
                notifyIcon.Icon = _blue;

                Logger.WriteLine("Sending API Request:");
                Logger.WriteLine($"   Url: {url}");
                outputTextBox.Text = "Sending API Request...";

                var startTimestamp = DateTime.Now;
                var request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(dataStream);
                string responseString = sr.ReadToEnd();

                if (responseString != null)
                {
                    //Console.WriteLine(responseString);
                    dynamic playerData = JsonConvert.DeserializeObject(responseString);
                    int rp = Convert.ToInt32(playerData.global.rank.rankScore);
                    var endTimestamp = DateTime.Now;

                    var interval = endTimestamp - startTimestamp;

                    Logger.WriteLine($"   Player: {playerData.global.name}");
                    Logger.WriteLine($"   Platform: {playerData.global.platform}");
                    Logger.WriteLine($"   BR Ranked Score: {playerData.global.rank.rankScore}");
                    Logger.WriteLine($"   AR Ranked Score: {playerData.global.arena.rankScore}");
                    Logger.WriteLine($"   Response Time: {interval.TotalMilliseconds}");
                    Logger.WriteLine($"   Version: {_apiVersion}");
                    Logger.WriteLine($"   Access Type: {playerData.mozambiquehere_internal.APIAccessType}");
                    Logger.WriteLine($"   Cluster ID: {playerData.mozambiquehere_internal.ClusterID}");
                    Logger.WriteLine($"   Cluster Srv: {playerData.mozambiquehere_internal.clusterSrv}");

                    string format = $"Pulled ranked score \"{playerData.global.rank.rankScore}\" from player \"{playerData.global.name}\" in {interval.TotalMilliseconds} ms";
                    outputTextBox.Text = format;
                    notifyIcon.Icon = start;
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
                MessageBox.Show("Player not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Exception(ex);
                return null;
            }
        }

        void LoadFromFiles()
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

        void Reset()
        {
            _kills = 0;
            _wins = 0;
            _rp = 0;

            WriteKillsToFile(0);
            WriteWinsToFile(0);
            WriteRPToFile(0);
        }

        void SetKillWins(dynamic data)
        {
            if (data != null)
            {
                if (data.Count >= 1)
                {
                    string legend_name1 = data[0].name;
                    string legend_value1 = data[0].value;

                    if (legend_name1.ToLower().Contains("kills"))
                    {
                        killsUpDown.Value = Convert.ToInt32(legend_value1);
                    }
                    else if (legend_name1.ToLower().Contains("wins"))
                    {
                        winsUpDown.Value = Convert.ToInt32(legend_value1);
                    }
                }

                if (data.Count >= 2)
                {
                    string legend_name2 = data[1].name;
                    string legend_value2 = data[1].value;

                    //Console.WriteLine(legend_name2);
                    //Console.WriteLine(legend_value2);

                    if (legend_name2.ToLower().Contains("kills"))
                    {
                        killsUpDown.Value = Convert.ToInt32(legend_value2);
                    }
                    else if (legend_name2.ToLower().Contains("wins"))
                    {
                        winsUpDown.Value = Convert.ToInt32(legend_value2);
                    }
                }

                if (data.Count >= 3)
                {
                    string legend_name3 = data[2].name;
                    string legend_value3 = data[2].value;

                    //Console.WriteLine(legend_name3);
                    //Console.WriteLine(legend_value3);

                    if (legend_name3.ToLower().Contains("kills"))
                    {
                        killsUpDown.Value = Convert.ToInt32(legend_value3);
                    }
                    else if (legend_name3.ToLower().Contains("wins"))
                    {
                        winsUpDown.Value = Convert.ToInt32(legend_value3);
                    }
                }
            }
        }

        void UpdateMatchHistory(dynamic playerData)
        {
            try
            {
                if (playerData != null)
                {
                    int rpDiff = 0;
                    int rp = 0;
                    int totalRp = 0;
                    int newRPDis = 0;

                    if (modeComboBox.SelectedIndex == 0)
                    {
                        FindDistance(Convert.ToInt32(playerData.global.rank.rankScore));
                        rpDiff = Convert.ToInt32(playerData.global.rank.rankScore - autoRPUpDown.Value);
                        rp = Convert.ToInt32(playerData.global.rank.rankScore) - _matchRP;
                        totalRp = Convert.ToInt32(playerData.global.rank.rankScore);
                        newRPDis = _rpDistance - totalRp;
                    }
                    else if (modeComboBox.SelectedIndex == 1)
                    {
                        FindDistance(Convert.ToInt32(playerData.global.arena.rankScore));
                        rpDiff = Convert.ToInt32(playerData.global.arena.rankScore - autoRPUpDown.Value);
                        rp = Convert.ToInt32(playerData.global.arena.rankScore) - _matchRP;
                        totalRp = Convert.ToInt32(playerData.global.arena.rankScore);
                        newRPDis = _rpDistance - totalRp;
                    }

                    if(_prevLegend == (string)playerData.legends.selected.LegendName)
                    {
                        UpdateKillWinValue(playerData.legends.selected.data);
                    }
                    else
                    {
                        SetKillWins(playerData.legends.selected.data);
                    }

                    _matchRP = Convert.ToInt32(playerData.global.rank.rankScore);

                    if (rp != _rp && rp != 0)
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

                        int rowNum = matchHistory.Rows.Add(modeComboBox.Text, playerData.realtime.selectedLegend, _kills, rpString, rpDiff.ToString(), totalRp.ToString());
                        DataGridViewRow row = matchHistory.Rows[rowNum];
                        matchHistory.FirstDisplayedScrollingRowIndex = matchHistory.RowCount - 1;

                        if (rp > 0)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Green;
                        }
                        else if (rp <= 0)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Red;
                        }
                    }

                    _rp = rpDiff;
                    WriteRPToFile(_rp);

                    
                    if(newRPDis >= 0)
                    {
                        WriteRPDistance(newRPDis);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        void UpdateKillWinValue(dynamic data)
        {
            int killStart = (int)killsUpDown.Value;
            int winStart = (int)winsUpDown.Value;

            int killsDiff = 0;
            if (data.Count >= 1)
            {
                killsDiff = Convert.ToInt32(data[0].value) - killStart;
            }

            int winsDiff = 0;
            if (data.Count >= 2)
            {
                winsDiff = Convert.ToInt32(data[1].value) - winStart;
            }

            _kills += killsDiff;
            _wins += winsDiff;

            WriteKillsToFile(_kills);
            WriteWinsToFile(_wins);

            SetKillWins(data);
        }

        void WriteKillsToFile(int value)
        {
            string format = killsFormatTB.Text;
            string valueString = value.ToString();
            format = format.Replace("$kills", valueString);

            liveKillsText.Text = format;
            using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\Kills.txt"))
            {
                sw.WriteLine(format);
            }
        }

        void WriteWinsToFile(int value)
        {
            string format = winsFormatTB.Text;
            string valueString = value.ToString();
            format = format.Replace("$wins", valueString);

            liveWinsText.Text = format;
            using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\Wins.txt"))
            {
                sw.WriteLine(format);
            }
        }

        void WriteRPToFile(int value)
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

        void WriteRPDistance(int value)
        {
            string format = rpDisFormatText.Text;

            string valueString = value.ToString();
            format = format.Replace("$rp", valueString);
            format = format.Replace("$rank", rankedTier[index]);

            distanceRPTextBox.Text = format;
            using (StreamWriter sw = new StreamWriter(_directory + @"\TextFiles\RP-Distance.txt"))
            {
                sw.WriteLine(format);
            }
        }

        #endregion

        private void MainForm_Shown(object sender, EventArgs e)
        {
            LoadAPI();
            CheckVersion();
        }

        private void killstxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader srKills = new StreamReader(_directory + @"\TextFiles\Kills.txt");
                _kills = Convert.ToInt32(srKills.ReadLine());
                srKills.Close();
                WriteKillsToFile(_kills);
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
                WriteWinsToFile(_wins);
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

        private void showFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(_directory);
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start($"https://portal.apexlegendsapi.com/");
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start($"https://portal.apexlegendsapi.com/dashboard");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                var player = GetPlayerData("https://api.mozambiquehe.re/bridge?version=" + _apiVersion + "&platform=" + platformCB.Text + "&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
                if (player != null)
                {
                    if (modeComboBox.SelectedIndex == 0)
                    {
                        FindDistance(Convert.ToInt32(player.global.rank.rankScore));
                        autoRPUpDown.Value = Convert.ToInt32(player.global.rank.rankScore);
                        _matchRP = Convert.ToInt32(player.global.rank.rankScore);
                    }
                    else if (modeComboBox.SelectedIndex == 1)
                    {
                        FindDistance(Convert.ToInt32(player.global.arena.rankScore));
                        autoRPUpDown.Value = Convert.ToInt32(player.global.arena.rankScore);
                        _matchRP = Convert.ToInt32(player.global.arena.rankScore);
                    }

                    if (player.legends.selected.data != null)
                    {
                        _prevLegend = player.legends.selected.LegendName;
                        SetKillWins(player.legends.selected.data);
                    }
                    else
                    {
                        MessageBox.Show("Unable to set stats because no trackers are equipped", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Exception(ex);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var player = GetPlayerData("https://api.mozambiquehe.re/bridge?version=" + _apiVersion + "&platform=" + platformCB.Text + "&player=" + usernameTextBox.Text + "&auth=" + apiKeyTextBox.Text);
            UpdateMatchHistory(player);            
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
                notifyIcon.Icon = _red;

                string format = $"Started auto refresh on interval {minutesUpDown.Value * 60} seconds";
                outputTextBox.Text = format;
            }
            else
            {
                button12.Enabled = true;
                loopTimer.Stop();
                notifyIcon.Icon = _gray;

                string format = $"Stopped auto refresh";
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
            Process.Start(Path.Combine(Logger.CurrentDirectory, "logs"));
        }

        private void viewCurrentLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Logger.CurrentDirectory, "logs", "latest.log"));
        }

        private void saveCurrentLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Save();
        }

        private void songFormatButton_Click(object sender, EventArgs e)
        {
            outputFormatText.Text = "$rp";
        }

        private void issuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/TheGuitarleader/ApexStreamDisplay/issues");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.TopMost = true;
                this.Focus();
                this.Show();
            }
        }

        private void matchHistory_SelectionChanged(object sender, EventArgs e)
        {
            matchHistory.ClearSelection();
        }

        private void rpDisFormatButton_Click(object sender, EventArgs e)
        {
            rpDisFormatText.Text = "$rank ($rp)";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("By Enigma Software\r\nPowered by https://apexlegendsapi.com/", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            killsFormatTB.Text = "$kills";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            winsFormatTB.Text = "$wins";
        }
    }
}

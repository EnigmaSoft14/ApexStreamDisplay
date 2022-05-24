using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApexStreamDisplay
{
    public partial class UpdateForm : Form
    {
        public dynamic _request { get; set; }

        public UpdateForm(string json)
        {
            _request = JsonConvert.DeserializeObject(json);
            InitializeComponent();

            Console.WriteLine(_request);

            versionLB.Text = (string)_request[0].tag_name;
            changesTB.Text = (string)_request[0].body;
        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            Process.Start((string)_request[0].html_url);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AvgCutImage
{
    public partial class FmAbout : Form
    {
        public FmAbout()
        {
            InitializeComponent();
        }

        private void FmAbout_Load(object sender, EventArgs e)
        {
            lbVer.Text += Application.ProductVersion.Remove(3, 4);
        }

        private void FmAbout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void lbBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://xxcanghai.cnblogs.com/");
        }

        private void lbGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/xxcanghai");
        }
    }
}

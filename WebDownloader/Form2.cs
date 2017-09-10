using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebDownloader
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
            textBox1.AppendText(Properties.Settings.Default.downloadPath);
            textBox2.AppendText(Properties.Settings.Default.ffmpeg);
        }

        private void applyButton(object sender, EventArgs e)
        {
            Properties.Settings.Default.downloadPath = textBox1.Text;
            Properties.Settings.Default.ffmpeg = textBox2.Text;
            Properties.Settings.Default.Save();

            this.Close(); 
        }

        private void resetButton(object sender, EventArgs e)
        {
            Properties.Settings.Default.downloadPath = @"C:\";
            Properties.Settings.Default.ffmpeg = @"C:\ffmpeg\bin\ffmpeg.exe";
            Properties.Settings.Default.Save();

            textBox1.Clear();
            textBox1.AppendText(Properties.Settings.Default.downloadPath);
        }
    }
}

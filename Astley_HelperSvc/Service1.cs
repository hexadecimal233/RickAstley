using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using murrayju.ProcessExtensions;

namespace Astley_HelperSvc {
    public partial class Service1 : ServiceBase {
        public Service1() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Timer timer = new Timer(30 * 1000);
            timer.Elapsed += TryStartAstley;
            timer.Start();
        }

        private void TryStartAstley(object source, ElapsedEventArgs e) {
            Process[] ps = Process.GetProcessesByName("rick");
            if (ps.Length <= 0) ProcessExtensions.StartProcessAsCurrentUser(Environment.GetEnvironmentVariable("SYSTEMROOT") + "\\rick.exe");
        }
    }
}

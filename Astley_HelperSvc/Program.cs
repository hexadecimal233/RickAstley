using System.ServiceProcess;

namespace Astley_HelperSvc {
    static class Program {
        private static void Main() {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new Service1() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

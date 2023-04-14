using System.ComponentModel;
using System.Configuration.Install;

namespace Astley_HelperSvc {
    [RunInstaller(true)]
    public partial class Installer1 : Installer {
        public Installer1() {
            InitializeComponent();
        }
    }
}

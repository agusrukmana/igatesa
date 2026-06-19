using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace H2HCoreBanking.WinServices
{
    [RunInstaller(true)]
    public class WinServiceInstaller :  Installer
    {
        public WinServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Setup the Service Account type per your requirement
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = "iGate Sync Agent";
            serviceInstaller.DisplayName = "iGate Sync Agent";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.Description = "iGate Sync Agent. Jurnal Multi Rekening by PAP Divisi TIF. Build Aug.2024";
            
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}

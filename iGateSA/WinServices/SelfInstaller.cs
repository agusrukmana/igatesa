using System;
using System.Configuration.Install;
using System.Reflection;
using Util.App_Code;

namespace H2HCoreBanking.WinServices
{
    public class SelfInstaller
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;
        private static Logger lg = new Logger();
		private static String AppName = "iGate Sync Agent";

        public static bool InstallMe()
        {
            bool result;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] {
                    SelfInstaller._exePath
                });

                lg.INFO($"{AppName} :: Install as Windows Services SelfInstaller/InstallMe. Success");
            }
            catch (Exception e)
            {
                lg.ERR($"{AppName} :: Install as Windows Services SelfInstaller/InstallMe. Error. Exception : {e.Message} ");

                result = false;
                return result;
            }
            result = true;
            return result;
        }

        public static bool UninstallMe()
        {
            bool result;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] {
                    "/u", SelfInstaller._exePath
                });

                lg.INFO($"{AppName} :: Uninstall as Windows Services SelfInstaller/UninstallMe. Success");
            }
            catch (Exception e)
            {
                lg.ERR($"{AppName} :: Uninstall as Windows Services SelfInstaller/UninstallMe. Error. Exception : {e.Message} ");

                result = false;
                return result;
            }
            result = true;
            return result;
        }

    }
}

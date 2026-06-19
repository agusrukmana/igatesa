using System;
using System.ServiceProcess;
using Util.App_Code;
using Application;
using System.Configuration;
using H2HCoreBanking.WinServices;

static class RunEnvironment
{

    private static AppMain app;

    class Program
    {
        static void Main(string[] args)
        {
            Logger lg = new Logger();

            if ((!Environment.UserInteractive))
            {
                Program.RunAsAService();
            }
            else
            {
                if (args != null && args.Length > 0)
                {
                    if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase))
                    {
                        lg.INFO(">>> Install Windows Services....");
                        SelfInstaller.InstallMe();
                    }
                    else
                    {
                        if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase))
                        {
                            lg.INFO(">>> Remove Windows Services....");
                            SelfInstaller.UninstallMe();
                        }
                        else
                        {
                            lg.INFO(">>> Invalid argument....");
                            Console.WriteLine("Invalid argument!");
                        }
                    }
                }
                else
                {
                    Program.RunAsAConsole();
                }
            }
        }

        static void RunAsAConsole()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Logger lg = new Logger();
            lg.INFO("<<< App Run As CONSOLE... >>>");

            int intInterval = 10000;
            try
            {
                lg.INFO("== Init Configuration ==");
                lg.INFO("Config >> iGateSAServices.exe.config ");

                intInterval = int.Parse(ConfigurationManager.AppSettings["app_timer_interval_seconds"]);
                int limitRows = int.Parse(ConfigurationManager.AppSettings["app_limit_rows"]);
                string appStartTime = ConfigurationManager.AppSettings["app_start_time"];
                string appEndTime = ConfigurationManager.AppSettings["app_end_time"];
                int intervalDelay = int.Parse(ConfigurationManager.AppSettings["app_backdate_delay"]);

                lg.INFO($"Config.app_limit_rows             : {limitRows}");
                lg.INFO($"Config.app_timer_interval_seconds : {intInterval / 1000} seconds");
                lg.INFO($"Config.app_start_time             : {appStartTime}:00 WITA");
                lg.INFO($"Config.app_end_time               : {appEndTime}:59 WITA");
                lg.INFO($"Config.app_backdate_delay         : {intervalDelay} minutes");


                lg.INFO("== End ==");
            }
            catch (Exception ex)
            {
                lg.ERR($"Invalid Parameter : app_timer_interval_seconds >> Expected Number. Found: {ConfigurationManager.AppSettings["app_timer_interval_seconds"]}. Exception : {ex.Message}");
            }

            /* Looping */
            loop:
                app = new AppMain();
                System.Threading.Thread.Sleep(intInterval);
            goto loop; 
            
        }

        static void RunAsAService()
        {
            ServiceBase[] servicesToRun = new ServiceBase[]
           {
                new WinService()
           };
            ServiceBase.Run(servicesToRun);
        }


    }
 
}

using System;
using System.ServiceProcess;
using Util.App_Code;
using Application;
using System.Timers;
using System.Configuration;

namespace H2HCoreBanking.WinServices
{
    partial class WinService : ServiceBase
    {       
        public WinService()
        {
            InitializeComponent();
        }

        public static AppMain app;
        public static Logger lg = new Logger();

        private Timer timer = new Timer();
        private int intInterval = 5000;

        protected override void OnStop()
        {
            timer.Stop();
            timer.Enabled = false;
            lg.INFO("OnStop", "", "Services Stop");
        }

        protected override void OnStart(string[] args)
        {
            lg.INFO("Run As Windows Services : Services started...");
            try
            {
                intInterval = int.Parse(ConfigurationManager.AppSettings["app_timer_interval_seconds"]);
                int limitRows = int.Parse(ConfigurationManager.AppSettings["app_limit_rows"]);

                lg.INFO($"Config >> app_limit_rows: {limitRows}");
                lg.INFO($"Config >> app_timer_interval_seconds: {intInterval / 1000} seconds");
            }
            catch (Exception ex)
            {
                lg.ERR($"Invalid Parameter : app_timer_interval_seconds >> Expected Number. Found: {ConfigurationManager.AppSettings["app_timer_interval_seconds"]}. Exception : {ex.Message}");
            }

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = intInterval;
            timer.Enabled = true;
            timer.Start();            

            Console.ReadLine();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {            
            app = new AppMain();
        }
    }
}

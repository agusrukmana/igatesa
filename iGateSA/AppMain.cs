using Application.Class;
using System;
using System.Configuration;
using System.Threading;
using Util.App_Code;

namespace Application
{
    public class AppMain
    {        
        /* Param */
        private static Logger log;
        private static TrxDataLog tig;
        private static BPJSTKPU bp;
        private static RefParameter rp;

        /* Main */
        public AppMain()
        {            
            tig = new TrxDataLog();
            bp = new BPJSTKPU();
            rp = new RefParameter();
            // Add other class here for multi purpose.

            RunEngine();                
        }

        /* Initial setting */
        public static void RunEngine()
        {
            /* Boleh Tx */
            int appStartTime = int.Parse(ConfigurationManager.AppSettings["app_start_time"]);
            int appEndTime = int.Parse(ConfigurationManager.AppSettings["app_end_time"]);
            int curTime = DateTime.Now.Hour;
            if (curTime >= appStartTime && curTime <= appEndTime)
            {
                /* Allow */
            }
            else
            {
                return;
            }

            /* Get data */
            log = new Logger();
            int limitRows = int.Parse(ConfigurationManager.AppSettings["app_limit_rows"]);

            var tg = new TrxDataLog();
            var list = tg.GetListUnproccess(limitRows);
			var idx = 1 ;
			
            foreach (TrxDataLog tx in list)
            {
                /* Split trx BPJSTK PU */
                if (tx.TransactionData.Contains("BPJS_TK_PU") || tx.TransactionData.Contains("BPJS PU"))
                {
					log.TRACE($"Records  :: #{idx++}");
                    log.TRACE($"TrxiGate :: {tx.Id} | {tx.Created.ToString("yyyy-MM-dd HH:mm:ss.fffff")} > {tx.TransactionData}");

                    /* Breakdown Json TransactionData, Do Processing and Flagging */
                    bp.DoBreakdown(tx);

                }
                else
                {
                    /* Do Nothing */
                }
                
            }
        }
    }

}
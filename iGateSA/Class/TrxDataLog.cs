/* Dev by Ekgun 19.08.2024 
 * Class ini digunakan untuk get list trx log success dari igate.
 * 2024.08.19 Init function, get trx from igate.
 */

using System;
using System.Collections.Generic;
using System.Data;
using DBase.App_Code;
using Util.App_Code;
using System.Configuration;

namespace Application.Class
{

    public class TrxDataLog
    {
        /* Raw Data */
        public decimal Id { get; set; }
        public string TransactionData { get; set; }
        public DateTime Created { get; set; }
        public string DbSource { get; set; }

        /* Response */
        public string FlgProces { get; set; }
        public string RCode{ get; set; }

        /* Refference */
        public string RCMessage{ get; set; }
        public string ReferenceNumber { get; set; }

        /* Parameter */
        Logger lg;
        TrxDataLog tg;
        DbPostgres db;
        DataTable dt;
        string sql;

        /* Database */
        string dbIgate = "db_igate";
        string dbIBB = "db_ibb";


        /* Get List */
        public List<TrxDataLog> GetListUnproccess(int limitRows)
        {
            int intervalDelay = int.Parse(ConfigurationManager.AppSettings["app_backdate_delay"]);
            sql = $" select id, transaction_data, created " + 
                   "   from t_transaction_data " + 
                   "  where flg_prcs = '0' " +
                  $"    and created >= NOW() - INTERVAL '{intervalDelay} minutes' " + 
                   "    and transaction_data like '%SUCCEED%' " + 
                  $"  limit {limitRows}; ";

            //Get DbIgate
            db = new DbPostgres(dbIgate);
            dt = db.GetRecord(sql);
            var listObject = new List<TrxDataLog>();           
            foreach (DataRow dr in dt.Rows)
            {
                tg = new TrxDataLog
                {
                    Id = Decimal.Parse(dr["id"].ToString()),
                    TransactionData = dr["transaction_data"].ToString(),
                    Created = (DateTime)dr["created"],
                    DbSource = dbIgate
                };

                listObject.Add(tg);
            }

            //Get DbIBB
            db = new DbPostgres(dbIBB);
            dt = db.GetRecord(sql);
            foreach (DataRow dr in dt.Rows)
            {
                tg = new TrxDataLog
                {
                    Id = Decimal.Parse(dr["id"].ToString()),
                    TransactionData = dr["transaction_data"].ToString(),
                    Created = (DateTime)dr["created"],
                    DbSource = dbIBB
                };

                listObject.Add(tg);
            }
            return listObject;
        }

        /* DoFlagging after process /Ekgun  */
        public bool DoFlagging(TrxDataLog tx)
        {
            bool endCode = false;
            sql = $" update  t_transaction_data " +
                  $"    set  flg_prcs  = '1', " +
                  $"         rcode     = '{tx.RCode}', " +
                  $"         tsflging  = now(), " + 
                  $"         appsrvs   = 'igatesa', " + 
                  $"         rcmsg     = '{tx.RCMessage}' " + 
                  $"  where  id        = {tx.Id} " + 
                  $"    and  flg_prcs  = '0'; ";

            db = new DbPostgres(tx.DbSource);
            int rAff = db.ExecDML(sql);
            string endMsg = "-";
            if (rAff >= 1)
            {
                endMsg = "SUCCESS";
                endCode = true;
            }
            else
            {
                endMsg = "FAILED";
                endCode = false;
            }

            lg = new Logger();
            lg.INFO($" DoFlagging : {tx.ReferenceNumber} | {endMsg} ({rAff})");

            return endCode;
        }
    }
}

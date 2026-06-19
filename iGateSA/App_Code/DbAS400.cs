using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using Util.App_Code;

namespace DBase.App_Code
{
    public class DbAS400
    {
        //DbName
        private string strConnString = ConfigurationManager.ConnectionStrings["db_devisa"].ConnectionString;

        //Param
        private OdbcConnection con;
        private OdbcCommand cmd;
        private OdbcDataAdapter da;
        private DataTable dt;
        private string SQL = null;
        private Logger log;

        //Get Record
        public DataTable GetRecord(string sQuery)
        {
            con = new OdbcConnection(strConnString);
            da = new OdbcDataAdapter();
            cmd = new OdbcCommand(sQuery);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            dt = new DataTable();

            try
            {
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(dt);
                con.Close();
                cmd.Dispose();
                da.Dispose();
                con.Dispose();
                return dt;
            }
            catch (Exception ex)
            {
                con.Close();
                cmd.Dispose();
                da.Dispose();
                con.Dispose();
                //Log
                log = new Logger();
                log.ERR("DB_AS400/GetRecord", SQL, ex.Message);
                return null;
            }
        }


        //Insert, Update, Delete
        public bool ExecDML(string sQuery)
        {
            con = new OdbcConnection(strConnString);
            cmd = new OdbcCommand(sQuery);

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
                //Log
                log = new Logger();
                log.ERR("DB_AS400/ExecDML", sQuery, ex.Message);
                return false;
            }
        }

        /* Get Connection String by Ekgun 2024.08.27 */
        public string GetConectionString()
        {
            return strConnString;
        }
    }
}

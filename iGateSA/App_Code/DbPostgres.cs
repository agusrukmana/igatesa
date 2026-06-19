using System;
using System.Configuration;
using System.Data;
using Util.App_Code;
using Npgsql;

namespace DBase.App_Code
{
    public class DbPostgres
    {
        //DbName
        private string strConnString;

        //Param
        private NpgsqlConnection con;
        private NpgsqlCommand cmd;
        private NpgsqlDataAdapter da;
        private DataTable dt;
        private string SQL = null;
        private string RCodeInfo = string.Empty;
        private Logger log;
		
		// Constructor
        public DbPostgres(string connectionName)
        {
            if (ConfigurationManager.ConnectionStrings[connectionName] != null)
            {
                strConnString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
            else
            {
				//Log
                log = new Logger();
                log.ERR("DbPostgres/ExecDML", connectionName, "Connection name not found in configuration.");
                throw new ArgumentException($"Connection name '{connectionName}' not found in configuration.");
            }
        }

        //Get Record
        public DataTable GetRecord(string sQuery)
        {
            con = new NpgsqlConnection(strConnString);
            da = new NpgsqlDataAdapter();
            cmd = new NpgsqlCommand(sQuery);
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
                log.ERR("DbPostgres/GetRecord", SQL, ex.Message);
                return null;
            }
        }


        //Insert, Update, Delete
        public int ExecDML(string sQuery)
        {
            con = new NpgsqlConnection(strConnString);
            cmd = new NpgsqlCommand(sQuery);
            int iRowAffected = -1;

            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                iRowAffected = cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                cmd.Dispose();
                return iRowAffected;
            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
                //Log
                log = new Logger();
                log.ERR("DbPostgres/ExecDML", SQL, ex.Message);
                return -1;
            }
        }

        //Connection
        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(strConnString);
        }
    }
}

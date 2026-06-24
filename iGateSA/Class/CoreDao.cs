using System;
using System.Data;
using DBase.App_Code;
using System.Data.Odbc;
using Util.App_Code;

namespace Application.Class
{
    /* CoreDao conected to CBS AS400 by Ekgun 2024.10.22 Add Local Git */

    public class CoreDao
    {
        /* Business Data */
        public string JnsTrx { get; set; }
        public string KdVia { get; set; }
        public string NoReff { get; set; }
        public string NoArsip { get; set; }
        public string LokTx { get; set; }
        public string KdValuta { get; set; }       
        public string ListTxDb { get; set; }
        public string ListTxCr { get; set; }        
        public string App { get; set; }        
        public string TglSys { get; set; }
        public string JamSys { get; set; }
        public string AddInfo { get; set; }
        public enum EnumFlgKoreksi { NORMAL = '0', REVERSAL = 'R' }
        public enum EnumProcBy { TELLER = '1', AUTOPROC = '3', WINSERVICES = '5' }        
        public EnumProcBy ProcBy;
        public EnumFlgKoreksi FlgKoreksi;

        /* Resp CBS */
        public double OdoStatus { get; set; }
        public string RCode { get; set; }
        public string Pesan { get; set; }

        /* Parameter */ 
        DbAS400 db;
        Logger lg;

        /* Call SP Insert Multi Rek*/
        public CoreDao DoInsertTxMultiRek(CoreDao cd)
        {
            // Logger
            lg = new Logger();

            // Init connection 
            using (OdbcConnection OdbcCon = new OdbcConnection())
            using (OdbcCommand OdbcCmd = new OdbcCommand())
            {
                db = new DbAS400();
                OdbcCon.ConnectionString = db.GetConectionString();
                OdbcCon.ConnectionTimeout = 1;

                // Stored Procedure
                OdbcCmd.CommandText = "{CALL MASTER.SP_TXMULTI_INSERT(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)}";
                OdbcCmd.CommandType = CommandType.StoredProcedure;
                OdbcCmd.Connection = OdbcCon;
                OdbcCmd.Parameters.Clear();

                // Parameter In SP. Penting. Urutan Parameters harus sesuai dengan SP CBS.
                OdbcCmd.Parameters.Add("ODO_STATUS", OdbcType.Double, 8).Direction = ParameterDirection.Output;
                OdbcCmd.Parameters.Add("IVC_JNSTX", OdbcType.VarChar, 50).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_KDVIA", OdbcType.Char, 3).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IVC_REFNO", OdbcType.VarChar, 30).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IVC_NOARSIP", OdbcType.Char, 10).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_LOKTX", OdbcType.Char, 3).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_KDVAL", OdbcType.Char, 3).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IVC_DB", OdbcType.VarChar, 32000).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IVC_KR", OdbcType.VarChar, 32000).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_APP", OdbcType.Char, 5).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_PROCBY", OdbcType.Char, 1).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_TGLSYS", OdbcType.Char, 8).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_JAMTX", OdbcType.Char, 6).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IC_FLGKOR", OdbcType.Char, 1).Direction = ParameterDirection.Input;
                OdbcCmd.Parameters.Add("IVC_ADDINFO", OdbcType.VarChar, 1024).Direction = ParameterDirection.Input;

                // Value IN
                OdbcCmd.Parameters["IVC_JNSTX"].Value = cd.JnsTrx;
                OdbcCmd.Parameters["IC_KDVIA"].Value = cd.KdVia;
                OdbcCmd.Parameters["IVC_REFNO"].Value = cd.NoReff;
                OdbcCmd.Parameters["IVC_NOARSIP"].Value = cd.NoArsip;
                OdbcCmd.Parameters["IC_LOKTX"].Value = cd.LokTx;
                OdbcCmd.Parameters["IC_KDVAL"].Value = cd.KdValuta;
                OdbcCmd.Parameters["IVC_DB"].Value = cd.ListTxDb;
                OdbcCmd.Parameters["IVC_KR"].Value = cd.ListTxCr;
                OdbcCmd.Parameters["IC_APP"].Value = cd.App;
                OdbcCmd.Parameters["IC_PROCBY"].Value = (char)cd.ProcBy;
                OdbcCmd.Parameters["IC_TGLSYS"].Value = cd.TglSys;
                OdbcCmd.Parameters["IC_JAMTX"].Value = cd.JamSys;
                OdbcCmd.Parameters["IC_FLGKOR"].Value = (char)cd.FlgKoreksi;
                OdbcCmd.Parameters["IVC_ADDINFO"].Value = cd.AddInfo;

                // Parameter Output SP                
                OdbcCmd.Parameters.Add("OC_RCODE", OdbcType.Char, 2).Direction = ParameterDirection.Output;
                OdbcCmd.Parameters.Add("OVC_PESAN", OdbcType.VarChar, 255).Direction = ParameterDirection.Output;

                // Logging & Tracing
                lg.TRACE($"  |- ODO_STATUS  : {cd.OdoStatus}");
                lg.TRACE($"  |- IVC_JNSTX   : {cd.JnsTrx}");
                lg.TRACE($"  |- IC_KDVIA    : {cd.KdVia}");
                lg.TRACE($"  |- IVC_REFNO   : {cd.NoReff}");
                lg.TRACE($"  |- IVC_NOARSIP : {cd.NoArsip}");
                lg.TRACE($"  |- IC_LOKTX    : {cd.LokTx}");
                lg.TRACE($"  |- IC_KDVAL    : {cd.KdValuta}");
                lg.TRACE($"  |- IVC_DB      : {cd.ListTxDb}");
                lg.TRACE($"  |- IVC_KR      : {cd.ListTxCr}");
                lg.TRACE($"  |- IC_APP      : {cd.App}");
                lg.TRACE($"  |- IC_PROCBY   : {cd.ProcBy}");
                lg.TRACE($"  |- IC_TGLSYS   : {cd.TglSys}");
                lg.TRACE($"  |- IC_JAMTX    : {cd.JamSys}");
                lg.TRACE($"  |- IC_FLGKOR   : {cd.FlgKoreksi}");
                lg.TRACE($"  |- IVC_ADDINFO   : {cd.AddInfo}");

                lg.REQ($"  CALL MASTER.SP_TXMULTI_INSERT(?,'{cd.JnsTrx}','{cd.KdVia}','{cd.NoReff}','{cd.NoArsip}', '{cd.LokTx}','{cd.KdValuta}','{cd.ListTxDb}','{cd.ListTxCr}','{cd.App}','{(char)cd.ProcBy}','{cd.TglSys}','{cd.JamSys}','{(char)cd.FlgKoreksi}',?,?)");

                // Connect Database
                try
                {
                    OdbcCon.Open();
                    // Generate No Reff baru TAMBHAKAN JIKA DIBUTHKAN KONDISI TERDAPAT NOREFF YANG SAMA PADA TGLTX
                    //cd.NoReff = GenerateRefNo(OdbcCon, cd.NoReff);
                    //OdbcCmd.Parameters["IVC_REFNO"].Value = cd.NoReff;
                    //lg.TRACE($"  ------------------|- NOREF  : {cd.NoReff}");
                    OdbcCmd.ExecuteNonQuery();
                    string sResponse = OdbcCmd.Parameters["OC_RCODE"].Value.ToString() + " : " +
                                       OdbcCmd.Parameters["OVC_PESAN"].Value.ToString() + " : (" +
                                       OdbcCmd.Parameters["ODO_STATUS"].Value.ToString() + ")";
                    lg.RESP($"Respon SP: {sResponse}");

                    cd.RCode = OdbcCmd.Parameters["OC_RCODE"].Value.ToString();
                    cd.Pesan = OdbcCmd.Parameters["OVC_PESAN"].Value.ToString();
                    cd.OdoStatus = Convert.ToDouble(OdbcCmd.Parameters["ODO_STATUS"].Value);

                    // Return
                    return cd;
                }
                catch (Exception ex)
                {
                    lg.ERR($"SP.Exception : {ex.Message} \n {ex.StackTrace}");

                    cd.RCode = "96";
                    cd.Pesan = "Exception. GAGAL CALL SP..!!!";
                    return cd ;
                }
                finally
                {
                    OdbcCon.Close();
                }

            }
            
        }

        private string GenerateRefNo(OdbcConnection conn, string refNo)
        {


            string sql = @"
        SELECT REFNO
        FROM MASTER.TX_MULTIREK
        WHERE TGLTX = (SELECT OPEN_DATE FROM MASTER.SYSTEM_HOST)
          AND REFNO LIKE ?
        ORDER BY REFNO DESC
        FETCH FIRST 1 ROW ONLY";

            using (OdbcCommand cmd = new OdbcCommand(sql, conn))
            {
                cmd.Parameters.Add("@P1", OdbcType.VarChar).Value = refNo + "%";

                object obj = cmd.ExecuteScalar();

                // belum ada
                if (obj == null || obj == DBNull.Value)
                    return refNo;

                string lastRef = obj.ToString();

                // REFNO asli tanpa suffix
                if (lastRef == refNo)
                    return refNo + "_01";

                int idx = lastRef.LastIndexOf('_');

                if (idx < 0)
                    return refNo + "_01";

                int counter = Convert.ToInt32(lastRef.Substring(idx + 1));

                return refNo + "_" + (counter + 1).ToString("D2");
            }
        }
    }
}

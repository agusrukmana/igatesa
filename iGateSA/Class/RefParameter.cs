using System.Data;
using DBase.App_Code;

namespace Application.Class
{
    public class RefParameter
    {
        // Data Notifikasi
        public string ParamName { get; set; }
        public string KdParam1 { get; set; }
        public string KdParam2 { get; set; }
        public string ParamValue { get; set; }
        public string Uraian { get; set; }
        public bool Success { get; set;  }

        // Local
        DataTable dt;
        string sql;

        // Get parameter Core @2023.04.10
        public RefParameter GetTblParamCBS(string KdParam1, string KdParam2)
        {
            sql = $"select kd_param1||kd_param2 as ref_param, value as param_value, keterangan from master.tbl_parameter where kd_param1 ='{KdParam1}' and kd_param2 = '{KdParam2}'";
            var dbAS400 = new DbAS400();
            dt = dbAS400.GetRecord(sql);
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow dr = dt.Rows[0];
            RefParameter obj;
            obj = new RefParameter
            {
                ParamName = dr["REF_PARAM"].ToString(),
                ParamValue = dr["PARAM_VALUE"].ToString(),
                Uraian = dr["KETERANGAN"].ToString()
            };

            return obj;
        }

        // Update parameter core by Ekgun @2024.10.18
        public RefParameter UpdateTblParam(RefParameter req)
        {
            sql =  $"Update master.tbl_parameter " + 
                   $"Set    value = '{req.ParamValue}' " + 
                   $"Where  kd_param1 = '{req.KdParam1}' " +
                   $"And    kd_param2 = '{req.KdParam2}' ";

            var dbAS400 = new DbAS400();
            if (dbAS400.ExecDML(sql))
            {
                req.Success = true;
            }
            else
            {
                req.Success = false;
            }

            return req;
        }
    }
}

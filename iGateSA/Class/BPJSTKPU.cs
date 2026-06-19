using Newtonsoft.Json;
using System;
using System.Globalization;
using Util.App_Code;

namespace Application.Class
{
    /* ToDo : Breakdown Json TransactionData, Do Processing and Do Flagging /ekgun 2024.10.22 */
    
    public class BPJSTKPU
    {
        /* Busines Data */
        public string ProductName { get; set; }
        public string ReferenceNumber { get; set; }
        public string CustomerReference { get; set; }
        public string Status { get; set; }
        public decimal BpjskTk { get; set; }
        public decimal Jht { get; set; }
        public decimal Jkk { get; set; }
        public decimal Jkm { get; set; }
        public decimal Jpk { get; set; }
        public decimal Jpn { get; set; }
        public string AccountNumber { get; set; }
        public string KodeCabang { get; set; }
        public string Description { get; set; }
        public string DeliveryChannel { get; set; }
        public string TrxVia { get; set; }
        public string TransactionDate { get; set; }
        public DateTime TransDate { get; set; }
        public StatusCode endCode { get; set; }
        
        /* Response Staus */
        public enum StatusCode
        {
            FlaggingSuccess = 0,
            InvalidAmountBreakdown = 10,
            UndefinedError = 99
        }

        /* Param */
        private static Logger log;
        private static CoreDao cd;

        /* Init */
        public BPJSTKPU()
        {
            log = new Logger();
        }

        /* Breakdown Json TransactionData */
        public bool DoBreakdown(TrxDataLog tx)
        {
            // Json string to class BPJSTKPU 
            string jsonString = tx.TransactionData;
            BPJSTKPU bp = JsonConvert.DeserializeObject<BPJSTKPU>(jsonString);

            // Cast
            bp.TransDate = DateTime.ParseExact(bp.TransactionDate, "dd MM yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            // Mapping
            switch (bp.DeliveryChannel)
            {
                case "11":
                case "12":
                    bp.DeliveryChannel = "MB";
                    break;
                case "IB":
                    bp.DeliveryChannel = "IBB";
                    break;
                case "TL":
                    bp.DeliveryChannel = "TLR";
                    break;
                default:
                    break;
            }

            log.INFO($" NoArsip: {bp.ReferenceNumber}");
            log.INFO($" - productName       : {bp.ProductName}");
            log.INFO($" - description       : {bp.Description}");
            log.INFO($" - accountNumber     : {bp.AccountNumber}");
            log.INFO($" - kodeCabang        : {bp.KodeCabang}");
            log.INFO($" - deliveryChannel   : {bp.DeliveryChannel}");
            log.INFO($" - customerReference : {bp.CustomerReference}");
            log.INFO($" - referenceNumber   : {bp.ReferenceNumber}");
            log.INFO($" - transactionDate   : {bp.TransDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            log.INFO($" - status            : {bp.Status}");
            log.INFO($" --- Total : Rp. {bp.BpjskTk.ToString("N0")}");
            log.INFO($" --- Jht   : Rp. {bp.Jht.ToString("N0")}");
            log.INFO($" --- Jkk   : Rp. {bp.Jkk.ToString("N0")}");
            log.INFO($" --- Jkm   : Rp. {bp.Jkm.ToString("N0")}");
            log.INFO($" --- Jpk   : Rp. {bp.Jpk.ToString("N0")}");
            log.INFO($" --- Jpn   : Rp. {bp.Jpn.ToString("N0")}");

            // Validate Total vs Jht, Jkk, Jkm, Jpk, Jpn 
            if ((bp.Jht + bp.Jkk + bp.Jkm + bp.Jpk + bp.Jpn) != bp.BpjskTk)
            {
                bp.endCode = StatusCode.InvalidAmountBreakdown;      
            }
            else
            {
                bp.endCode = StatusCode.FlaggingSuccess;
            }

            tx.RCode = ((int)bp.endCode).ToString();
            tx.RCMessage = bp.endCode.ToString();
            tx.ReferenceNumber = bp.ReferenceNumber;

            log.INFO($" --- RCode     : {tx.RCode}");
            log.INFO($" --- RCMessage : {tx.RCMessage}");

            // Call SP Insert TxMulti Rek
            log.REQ($"Core.CallSP.SP_TXMULTI_INSERT|{bp.Description}|{bp.ReferenceNumber}, KdValuta(360),Db({bp.BpjskTk}),Kr({bp.Jht},{bp.Jkk},{bp.Jkm},{bp.Jpk},{bp.Jpn})");

            // Mapping to CoreDao 
            cd            = new CoreDao();
            cd.JnsTrx     = bp.Description;
            cd.KdVia      = bp.DeliveryChannel; // Perlu mapping based on Description atau lainnya
            cd.NoReff     = $"{bp.CustomerReference}.{bp.ReferenceNumber}";
            cd.NoArsip    = bp.ReferenceNumber;
            cd.LokTx      = bp.KodeCabang;
            cd.KdValuta   = "360";
            cd.ListTxDb   = bp.BpjskTk.ToString("F2");
            cd.ListTxCr   = $"{bp.Jht.ToString("F2")}|{bp.Jkk.ToString("F2")}|{bp.Jkm.ToString("F2")}|{bp.Jpk.ToString("F2")}|{bp.Jpn.ToString("F2")}";
            cd.App        = "IGATE";
            cd.ProcBy     = CoreDao.EnumProcBy.WINSERVICES;
            cd.FlgKoreksi = CoreDao.EnumFlgKoreksi.NORMAL;
            cd.TglSys     = bp.TransDate.ToString("yyyyMMdd");
            cd.JamSys     = bp.TransDate.ToString("HHmmss");
            cd.DoInsertTxMultiRek(cd);

            string endCodeSP    = cd.RCode;
            string endCodePesan = cd.Pesan;

            // Do Flagging 
            tx.RCode     = cd.RCode;
            tx.RCMessage = cd.Pesan;
            log.INFO($" - {cd.RCode}|{cd.Pesan}");
            if (tx.RCode == "89")
            {   // Transaksi tidak diproses. EOD atau lainnya di CBS
                return false;
            }

            bool endCore = tx.DoFlagging(tx);            
            if(!endCore)
            {
                // Gagal flagging
                cd.FlgKoreksi = CoreDao.EnumFlgKoreksi.REVERSAL;
                cd.DoInsertTxMultiRek(cd);
            }       
            
            return false;
        }

    }

    

}

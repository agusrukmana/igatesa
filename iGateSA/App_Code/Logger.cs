using System;
using System.Configuration;
using System.IO;
using Telegram.Bot.Types.Enums;

namespace Util.App_Code
{
    public class Logger
    {
        //Parameter
        private string spliter = "\t";
        private Telegram.Bot.TelegramBotClient tBot;

        //Write text
        public void Log(string type, string modul, string input, string response)
        {
            if (Environment.UserInteractive)
            {
                //Run as ConsoleApp
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));

                if(type=="INFO")
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                else if(type.Contains("REQ"))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if(type.Contains("RESP"))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if(type.Contains("ERR"))
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if(type.Contains("WARN"))
                {
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (type.Contains("TRACE"))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                               
                Console.Write($" {type} ");
                Console.BackgroundColor = ConsoleColor.Black;

                // Formating ForegroundColor
                if (type.Contains("REQ") || type.Contains("RESP"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (type.Contains("ERR"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine(" {0} {1} {2} ", modul, input, response);
            }

            try
            {
                string pathlog = ConfigurationManager.AppSettings["log_path"].ToString();
                if (!Directory.Exists(pathlog))
                {
                    Directory.CreateDirectory(pathlog);
                }

                string logFile = ConfigurationManager.AppSettings["log_path"].ToString() + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                using (StreamWriter writer = new StreamWriter(logFile, true))
                {
                    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    writer.Write(spliter);
                    writer.Write(type);
                    writer.Write(spliter);
                    writer.Write(modul);
                    writer.Write(spliter);
                    writer.Write(input);
                    writer.Write(spliter);
                    writer.Write(response);
                    writer.WriteLine();
                }

                // -- Telegram
                var isLogError = Convert.ToInt32(ConfigurationManager.AppSettings["telegram_notify_error"].ToString());
                var isLogInfo = Convert.ToInt32(ConfigurationManager.AppSettings["telegram_notify_info"].ToString());

                string sTargetID = ConfigurationManager.AppSettings["telegram_chat_id"];
                string sToken = ConfigurationManager.AppSettings["telegram_bot_token"];

                if (isLogError == 1 & type == "ERROR")
                    SendText(sTargetID, "*ERROR : " + modul + "* \n *Input:* " + input.Replace("_", "-") + "\n" + "*Response:* " + response.Replace("_", "-"), sToken);
                else if (isLogInfo == 1 & type == "INFO")
                    SendText(sTargetID, "*INFO : " + modul + "*" + "\n" + "*Input:* " + input.Replace("_", "-") + "\n" + "*Response:* " + response.Replace("_", "-"), sToken);
            }
            catch (Exception ex)
            {
                string logFile = ConfigurationManager.AppSettings["log_path"].ToString() + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                using (StreamWriter writer = new StreamWriter(logFile, true))
                {
                    writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + ex.Message);
                }
            }
        }

        //Info
        public void INFO(string modul="", string input="", string response="")
        {
            Log("INFO", modul, input, response);
        }

        //Warning
        public void WARN(string modul = "", string input = "", string response = "")
        {
            Log("WARN", modul, input, response);
        }

        //Error
        public void ERR(string modul="", string input="", string response="")
        {
            Log("ERR ", modul, input, response);
        }

        //Trace
        public void TRACE(string data)
        {
            Log("TRACE", data, "", "");
        }
        public void REQ(string data)
        {
            Log("REQ ", $"> {data}", "", "");
        }
        public void RESP(string data)
        {
            Log("RESP", $"< {data}", "", "");
        }

        //Send Text Telegram
        public void SendText(string sToChatID, string sPesan, string sTokenBot)
        {
            tBot = new Telegram.Bot.TelegramBotClient(sTokenBot);
            tBot.SendTextMessageAsync(sToChatID, sPesan, ParseMode.Markdown);
        }
    }
}

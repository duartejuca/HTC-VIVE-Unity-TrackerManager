using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Logger
{
    public static class LogTool
    {
        private static readonly string FilePath = "log.xml";
        private static readonly LogData LogData = new LogData();

        public static void RegisterLog(string logInformation, bool removeDate = false)
        {
            DateTime now = DateTime.Now;
            LogData.id++;
            LogData.Information.Add($"{(removeDate ? ' ': now) } | {LogData.id} \n{logInformation}");
            SaveData();
        }

        private static void SaveData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LogData));
            using (FileStream stream = new FileStream(FilePath, FileMode.Create))
            {
                serializer.Serialize(stream, LogData);
            }
        }
    }

    [System.Serializable]
    public class LogData
    {
        public int id;
        public List<string> Information = new List<string>();
    }
}

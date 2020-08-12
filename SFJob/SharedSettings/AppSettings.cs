using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharedSettings
{
    class AppSettings
    {
        public static void SettConfig(Settings settings)
        {
            FileInfo settingsFile;
            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string filename = String.Format(@"{0}.xml", "SFJobConfig");
            settingsFile = new FileInfo(Path.Combine(commonAppDataPath, "SFJob", filename));
            var serializer = new XmlSerializer(typeof(Settings));
            if (settingsFile.Directory != null && !settingsFile.Directory.Exists)
                settingsFile.Directory.Create();
            using (XmlWriter writer = new XmlTextWriter(File.Create(settingsFile.FullName), Encoding.UTF8))
            {
                serializer.Serialize(writer, settings);
            }
        }
        public static Settings GetConfig(Settings settings)
        {
            FileInfo settingsFile;
            string commonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string filename = String.Format(@"{0}.xml", "SFJobConfig");
            settingsFile = new FileInfo(Path.Combine(commonAppDataPath, "SFJob", filename));
            if (settingsFile.Directory != null && settingsFile.Directory.Exists)
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    using (XmlReader reader = new XmlTextReader(File.OpenRead(settingsFile.FullName)))
                    {
                        settings = (Settings)serializer.Deserialize(reader);
                    }
                }
                catch (Exception)
                {
                }
            }
            return settings;
        }
    }
    public class Settings
    {
        public string username
        { get; set; }
        public string password
        { get; set; }
        public string token
        { get; set; }
        public string clientId
        { get; set; }
        public string clientSecret
        { get; set; }
        public double tiempo
        { get; set; }
        public double job_intervalo
        { get; set; }
        public DateTime hora_ejec
        { get; set; }
        public bool editTiempo
        { get; set; }
        public string url_login
        { get; set; }
        public string url_clientes
        { get; set; }
        public string url_facturas
        { get; set; }
        public string url_viajes
        { get; set; }
        public string carpeta_lectura
        { get; set;}
        public string carpeta_log
        { get; set; }
        public string carpeta_correctos
        { get; set; }
        public List<string> errores
        { get; set; }
        public Settings()
        {
            List<string> errores1 = new List<string>();
            username = "";
            password = "";
            token = "";
            clientId = "";
            clientSecret = "";
            url_login = "";
            url_clientes = "";
            url_facturas = "";
            url_viajes = "";
            tiempo = 24;
            job_intervalo = 0.5;
            editTiempo = false;
            hora_ejec = System.DateTime.Now;
            carpeta_lectura = @"C:\Shares\Saptxt";
            carpeta_log = @"C:\Shares\Saptxt\LOG\";
            carpeta_correctos = @"C:\Shares\Saptxt\OLD FILES SF\";
            errores1.Add("APEX_ERROR");
            errores1.Add("INVALID_SESSION");
            errores1.Add("E001");
            errores1.Add("E002");
            errores = errores1;
        }
    }
}

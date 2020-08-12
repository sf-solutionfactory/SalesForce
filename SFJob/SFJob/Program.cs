using Newtonsoft.Json;
using SFJob.Services;
using SharedSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SFJob
{
    class Program
    {
        static Settings config = new Settings();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static void Main(string[] args)
        {
            bool estado = true;
            double tiempo = 86400000;
            bool ejecutando = false;
            while (estado)
            {
                config = AppSettings.GetConfig(config);
                if (config.tiempo <= 0)
                {
                    estado = false;
                }
                else
                {
                    tiempo = config.tiempo * 3600000;
                }
                try
                {
                    JOBS(config, ejecutando);
                    ejecutando = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                System.Threading.Thread.Sleep(Convert.ToInt32(tiempo));
            }
            Console.ReadLine();
        }
        static void JOBS(Settings config, bool ejecutando)
        {
            var handle = GetConsoleWindow();
            Log log = new Log();
            SFRest client = new SFRest();
            string jsonLogin, tipoToken, autorizacionToken;
            List<FileInfo> archivos = new List<FileInfo>();
            List<string[]> contenidoLog = new List<string[]>();
            List<string> ins = new List<string>();
            List<string> act = new List<string>();
            List<string> eli = new List<string>();
            DateTime fecha;
            if (!config.editTiempo)
            { fecha = System.DateTime.Now.AddMinutes(5); }
            else
            { fecha = config.hora_ejec.AddMinutes(1); }
            int hora = Convert.ToInt32(fecha.ToString("%H"));
            int minuto = Convert.ToInt32(fecha.ToString("%m"));
            config.job_intervalo *= 60;
            jsonLogin = client.Login(config.clientId, config.clientSecret, config.username, config.password, config.token, config.url_login);
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonLogin);
            autorizacionToken = values["access_token"];
            tipoToken = values["token_type"] + " ";
            bool ban = true;
            if (ban)
            {
                if (!ejecutando)
                {
                    Job.IntervalInMinutes(hora, minuto, config.job_intervalo, () =>
                    {
                        archivos = log.LeeArchivos(config.carpeta_lectura, "I", false);
                        ins = client.InsertaRegistros(archivos, tipoToken, autorizacionToken, config.url_clientes, config.url_viajes, config.url_facturas, config.errores);
                    });
                    fecha = fecha.AddMinutes(5);
                    hora = Convert.ToInt32(fecha.ToString("%H"));
                    minuto = Convert.ToInt32(fecha.ToString("%m"));
                    Job.IntervalInMinutes(hora, minuto, config.job_intervalo, () =>
                    {
                        archivos = log.LeeArchivos(config.carpeta_lectura, "U", false);
                        act = client.ActualizaRegistros(archivos, tipoToken, autorizacionToken, config.url_clientes, config.url_viajes, config.url_facturas, config.errores);
                    });
                    fecha = fecha.AddMinutes(5);
                    hora = Convert.ToInt32(fecha.ToString("%H"));
                    minuto = Convert.ToInt32(fecha.ToString("%m"));
                    Job.IntervalInMinutes(hora, minuto, config.job_intervalo, () =>
                    {
                        archivos = log.LeeArchivos(config.carpeta_lectura, "C", false);
                        eli = client.CancelaRegistros(archivos, tipoToken, autorizacionToken, config.url_viajes, config.url_facturas, config.errores);
                    });
                    fecha = fecha.AddMinutes(5);
                    hora = Convert.ToInt32(fecha.ToString("%H"));
                    minuto = Convert.ToInt32(fecha.ToString("%m"));
                    Job.IntervalInMinutes(hora, minuto, config.job_intervalo, () =>
                    {
                        string fechaHora = DateTime.Now.ToString("dd-MM-yyyy H-mm-00");
                        contenidoLog.Clear();
                        contenidoLog = client.MuestraLista();
                        List<string> textoLog = Contenido(contenidoLog);
                        List<string> correctos = CorrectosLog(contenidoLog);
                        log.CreateEmptyDirectory(config.carpeta_log, "log" + fechaHora + ".txt", textoLog);
                        archivos = log.LeeArchivos(config.carpeta_lectura, "", true);
                        log.MueveArchivos(config.carpeta_correctos + fechaHora, archivos, correctos);
                    });
                }
            }
            else
            {
                archivos = log.LeeArchivos(config.carpeta_lectura, "I", false);
                ins = client.InsertaRegistros(archivos, tipoToken, autorizacionToken, config.url_clientes, config.url_viajes, config.url_facturas, config.errores);

                archivos = log.LeeArchivos(config.carpeta_lectura, "U", false);
                act = client.ActualizaRegistros(archivos, tipoToken, autorizacionToken, config.url_clientes, config.url_viajes, config.url_facturas, config.errores);

                archivos = log.LeeArchivos(config.carpeta_lectura, "C", false);
                eli = client.CancelaRegistros(archivos, tipoToken, autorizacionToken, config.url_viajes, config.url_facturas, config.errores);

                string fechaHora = DateTime.Now.ToString("dd-MM-yyyy H-mm-00");
                contenidoLog.Clear();
                contenidoLog = client.MuestraLista();
                List<string> textoLog = Contenido(contenidoLog);
                List<string> correctos = CorrectosLog(contenidoLog);
                log.CreateEmptyDirectory(config.carpeta_log, "log" + fechaHora + ".txt", textoLog);
                archivos = log.LeeArchivos(config.carpeta_lectura, "", true);
                log.MueveArchivos(config.carpeta_correctos + fechaHora, archivos, correctos);
            }
        }
        static List<string> Contenido(List<string[]> log)
        {
            List<string> contenidoLog = new List<string>();
            foreach (string[] item in log)
            {
                contenidoLog.Add(item[1]);
            }
            return contenidoLog;
        }
        static List<string> CorrectosLog(List<string[]> log)
        {
            List<string> contenidoLog = new List<string>();
            foreach (string[] item in log)
            {
                if (item[0] != null)
                { contenidoLog.Add(item[0]); }
            }
            return contenidoLog;
        }
    }
}

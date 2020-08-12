using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFJob.Services
{
    class Log
    {
        public void CreateEmptyDirectory(string rutaCarpeta, string nombreArchivo, List<string> contenido)
        {
            string fullPathArchivo = rutaCarpeta + nombreArchivo;
            try
            {
                if (!System.IO.Directory.Exists(rutaCarpeta))
                {
                    System.IO.Directory.CreateDirectory(rutaCarpeta);
                }
                if (System.IO.Directory.Exists(rutaCarpeta) & !System.IO.File.Exists(fullPathArchivo))
                {
                    StreamWriter sw = new StreamWriter(fullPathArchivo);
                    sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy H-mm-ss"));
                    foreach (string item in contenido)
                    {
                        sw.WriteLine(item);
                        sw.WriteLine("");
                    }
                    sw.Close();
                }
                else
                {
                    StreamWriter sw = new StreamWriter(fullPathArchivo, true);
                    sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy H-mm-ss"));
                    foreach (string item in contenido)
                    {
                        sw.WriteLine(item);
                        sw.WriteLine("");
                    }
                    sw.Close();
                }
            }
            catch (Exception) { }
        }
        public void MueveArchivos(string rutaCarpeta, List<FileInfo> archivos, List<string> correctos)
        {
            if (!System.IO.Directory.Exists(rutaCarpeta))
            {
                System.IO.Directory.CreateDirectory(rutaCarpeta);
            }
            foreach (FileInfo file in archivos)
            {
                if (correctos.Contains(file.Name))
                {
                    file.MoveTo(rutaCarpeta + "\\" + file.Name);
                }
            }
        }
        public List<FileInfo> LeeArchivos(string ruta, string tipoOperacion, bool full)
        {
            DirectoryInfo di = new DirectoryInfo(ruta);
            List<FileInfo> archivos = new List<FileInfo>();
            FileInfo[] getArchivos;
            getArchivos = di.GetFiles();
            if (getArchivos.Count() > 0)
            {
                foreach (FileInfo file in getArchivos)
                {
                    if (!full & tipoOperacion != "")
                    {
                        string nombreArchivo = file.Name;
                        string tipo = nombreArchivo.Substring(4, 1);
                        if (tipo == tipoOperacion)
                        {
                            archivos.Add(file);
                        }
                    }
                    else
                    {
                        archivos.Add(file);
                    }
                }
            }
            return archivos;
        }
    }
}

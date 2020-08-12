using SharedSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SFJob
{
    class SFRest
    {
        private readonly List<string[]> correctos = new List<string[]>();
        private readonly List<LogWS> retornoSAP = new List<LogWS>();
        static SFRest()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }
        public List<string[]> MuestraLista()
        {
            List<string[]> items = new List<string[]>();
            try
            {
                foreach (string[] item in correctos)
                {
                    items.Add(item);
                }
            }
            catch (Exception) { }
            finally
            {
                correctos.Clear();
            }
            return items;
        }
        public List<string> RetornaLista()
        {
            List<string> items = new List<string>();
            try
            {
                foreach (LogWS item in retornoSAP)
                {
                    string jsonString = JsonConvert.SerializeObject(item);
                    items.Add(jsonString);
                }
            }
            catch (Exception) { }
            finally
            {
                retornoSAP.Clear();
            }
            return items;
        }
        public string Login(string clientid, string clientsecret, string username, string password, string token, string url_login)
        {
            HttpClient apiCallClient = new HttpClient();
            Dictionary<string, string> parametros = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", clientid },
                { "client_secret", clientsecret },
                { "username", username },
                { "password", password + token }
            };
            FormUrlEncodedContent request = new FormUrlEncodedContent(parametros);
            request.Headers.Add("X-PrettyPrint", "1");
            var response = apiCallClient.PostAsync(url_login, request).Result;
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            return jsonResponse;
        }
        public List<string> InsertaRegistros(List<FileInfo> archivos, string tipoToken, string autorizacionToken, string url_clientes, string url_viajes, string url_facturas, List<string> errores)
        {
            List<string> respuestas = new List<string>();
            if (archivos.Count > 0 & tipoToken != "" & autorizacionToken != "")
            {
                foreach (FileInfo file in archivos)
                {
                    string fullPathName = file.FullName;
                    string nombreArchivo = file.Name;
                    string modulo = nombreArchivo.Substring(0, 3);
                    foreach (string body in File.ReadAllLines(fullPathName))
                    {
                        try
                        {
                            if (modulo == "CLI")
                            {
                                string cuerpo = body.Replace("\"CLIENTE\"", "\"Cliente\"").Replace("\"false\"", "false").Replace("\"true\"", "true");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_clientes, nombreArchivo, errores, "");
                            }
                            else if (modulo == "VIA")
                            {
                                Viaje vi = JsonConvert.DeserializeObject<Viaje>(body);
                                string cuerpo = body.Replace("\"NUEVOVIAJE\"", "\"NuevoViaje\"").Replace("\"CLIENTE\"", "\"cliente\"").Replace("\"PESO\":\"\"", "\"PESO\":null").Replace("\"RECUP_EVIDENCIA\":\"0000-00-00\"", "\"RECUP_EVIDENCIA\":null").Replace("\"PERSONA_RECIBE\":\"\"", "\"PERSONA_RECIBE\":null");
                                MetodoPost(tipoToken, autorizacionToken, cuerpo, url_viajes, nombreArchivo, errores, vi.NUEVOVIAJE.TRANSPORTE);
                            }
                            else if (modulo == "FAC")
                            {
                                string cuerpo = body.Replace("\"NEWFACTURA\"", "\"newFactura\"").Replace("\"CLIENTE\"", "\"cliente\"");
                                MetodoPost(tipoToken, autorizacionToken, cuerpo, url_facturas, nombreArchivo, errores, "");
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return respuestas;
        }
        public List<string> ActualizaRegistros(List<FileInfo> archivos, string tipoToken, string autorizacionToken, string url_clientes, string url_viajes, string url_facturas, List<string> errores)
        {
            List<string> respuestas = new List<string>();
            if (archivos.Count > 0 & tipoToken != "" & autorizacionToken != "")
            {
                foreach (FileInfo file in archivos)
                {
                    string fullPathName = file.FullName;
                    string nombreArchivo = file.Name;
                    string modulo = nombreArchivo.Substring(0, 3);
                    foreach (string body in File.ReadAllLines(fullPathName))
                    {
                        try
                        {
                            if (modulo == "CLI")
                            {
                                string cuerpo = body.Replace("\"CLIENTE\"", "\"Cliente\"").Replace("\"FALSE\"", "false").Replace("\"TRUE\"", "true");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_clientes, nombreArchivo, errores, "");
                            }
                            else if (modulo == "VIA")
                            {
                                Viaje vi = JsonConvert.DeserializeObject<Viaje>(body);
                                string cuerpo = body.Replace("\"VIAJEPROVECHOSO\"", "\"ViajeProvechoso\"");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_viajes, nombreArchivo, errores, vi.NUEVOVIAJE.TRANSPORTE);
                            }
                            else if (modulo == "FAC")
                            {
                                string cuerpo = body.Replace("\"REQUESTFACTURA\"", "\"requestFactura\"");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_facturas, nombreArchivo, errores, "");
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return respuestas;
        }
        public List<string> CancelaRegistros(List<FileInfo> archivos, string tipoToken, string autorizacionToken, string url_viajes, string url_facturas, List<string> errores)
        {
            List<string> respuestas = new List<string>();
            if (archivos.Count > 0 & tipoToken != "" & autorizacionToken != "")
            {
                foreach (FileInfo file in archivos)
                {
                    string fullPathName = file.FullName;
                    string nombreArchivo = file.Name;
                    string modulo = nombreArchivo.Substring(0, 3);
                    foreach (string body in File.ReadAllLines(fullPathName))
                    {
                        try
                        {
                            if (modulo == "VIA")
                            {
                                Viaje vi = JsonConvert.DeserializeObject<Viaje>(body);
                                string cuerpo = body.Replace("\"VIAJEPROVECHOSO\"", "\"ViajeProvechoso\"");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_viajes, nombreArchivo, errores, vi.NUEVOVIAJE.TRANSPORTE);
                            }
                            else if (modulo == "FAC")
                            {
                                string cuerpo = body.Replace("\"REQUESTFACTURA\"", "\"requestFactura\"");
                                MetodoPut(tipoToken, autorizacionToken, cuerpo, url_facturas, nombreArchivo, errores, "");
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return respuestas;
        }
        public async void MetodoPut(string token_type, string token, string body, string endPoint, string archivo, List<string> errores, string id)
        {
            HttpClient apiCallClient = new HttpClient();
            HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Put, endPoint);
            string[] respuesta = new string[2];
            LogWS logWS = new LogWS();
            logWS.ID = id;
            apiRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            apiRequest.Headers.Add("X-PrettyPrint", "1");
            apiRequest.Headers.Add("authorization", token_type + token);
            int rows = errores.Count;
            int i = 1;
            try
            {
                HttpResponseMessage apiCallResponse = await apiCallClient.SendAsync(apiRequest);
                string requestResponse = await apiCallResponse.Content.ReadAsStringAsync();
                foreach (string error in errores)
                {
                    if (requestResponse.Contains(error))
                    {
                        logWS.CODE = "ERROR";
                        retornoSAP.Add(logWS);

                        respuesta[0] = "ERROR_" + archivo;
                        return;
                    }
                    else if (i == rows)
                    {
                        logWS.CODE = "OK";
                        retornoSAP.Add(logWS);
                        respuesta[0] = archivo;
                    }
                    i++;
                }
                respuesta[1] = archivo + "\n" + requestResponse;
                correctos.Add(respuesta);
                Console.WriteLine("|********************************************************|");
                Console.WriteLine(archivo);
                Console.WriteLine(requestResponse);
                Console.WriteLine("\n");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }
        public async void MetodoPost(string token_type, string token, string body, string endPoint, string archivo, List<string> errores, string id)
        {
            HttpClient apiCallClient = new HttpClient();
            HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Post, endPoint);
            string[] respuesta = new string[2];
            LogWS logWS = new LogWS();
            logWS.ID = id;
            apiRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            apiRequest.Headers.Add("X-PrettyPrint", "1");
            apiRequest.Headers.Add("authorization", token_type + token);
            int rows = errores.Count;
            int i = 1;
            try
            {
                HttpResponseMessage apiCallResponse = await apiCallClient.SendAsync(apiRequest);
                string requestResponse = await apiCallResponse.Content.ReadAsStringAsync();
                foreach (string error in errores)
                {
                    if (requestResponse.Contains(error))
                    {
                        logWS.CODE = "ERROR";
                        retornoSAP.Add(logWS);
                        respuesta[0] = "ERROR_" + archivo;
                        return;
                    }
                    else if (i == rows)
                    {
                        logWS.CODE = "OK";
                        retornoSAP.Add(logWS);
                        respuesta[0] = archivo;
                    }
                    i++;
                }
                respuesta[1] = archivo + "\n" + requestResponse;
                correctos.Add(respuesta);

                Console.WriteLine("|********************************************************|");
                Console.WriteLine(archivo);
                Console.WriteLine(requestResponse);
                Console.WriteLine("\n");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
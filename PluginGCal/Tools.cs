using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Reflection;
using System.Collections;

namespace GCal
{
    public class Tools
    {
        public static Byte[] GetBytes(StringBuilder content)
        {
            return GetBytes(content.ToString());
        }
        
        public static Byte[] GetBytes(String content)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(content);

            return bytes;
        }


        public static String GetPageContent(String url, String contentType, String[][] headers)
        {
            return GetPageContent(url, "GET", contentType, headers, null);
        }

        public static String PostPageContent(String url, String contentType, String[][] headers, byte[] content)
        {
            return GetPageContent(url, "POST", contentType, headers, content);
        }


        public static String GetPageContent(String url, String method, String contentType, String[][] headers, byte[] content)
        {
            WebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            Stream dataStream = null;

            try
            {
                webRequest = WebRequest.Create(url);
                webRequest.Method = method;

                if (!String.IsNullOrEmpty(contentType))
                    webRequest.ContentType = contentType;

                if (headers != null)
                {
                    for (int i = 0; i < headers.Length; i++)
                    {
                        webRequest.Headers.Add(headers[i][0], headers[i][1]);
                    }
                }

                if (content != null)
                {
                    dataStream = webRequest.GetRequestStream();
                    dataStream.Write(content, 0, content.Length);
                    dataStream.Close();
                }


                webResponse = (HttpWebResponse)webRequest.GetResponse();
                String response = GetString(webResponse.GetResponseStream());

                if (webResponse.StatusCode == HttpStatusCode.Moved || webResponse.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    // Aller voir le header location
                    String location = webResponse.Headers["location"];
                    response = PostPageContent(location, contentType, headers, content);
                }

                return response;
            }
            finally
            {
                try { webResponse.Close(); } catch (Exception) { }
                try { dataStream.Close(); } catch (Exception) { }
            }
        }

        



        public static String GetString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


        public static string GetDateRFC3339(DateTime? date)
        {
            if (date != null)
            {
                //DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc((DateTime) dateTime);
                DateTime utcDateTime = (DateTime)date;
                return utcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                return "";
            }
        }


        public static DateTime GetDateRFC3339(String date)
        {
            return XmlConvert.ToDateTime(date);
        }


        public static Boolean IsToday(DateTime date)
        {
            DateTime now = DateTime.Now;
            return (date.Year == now.Year && date.Month == now.Month && date.Day == now.Day);
        }


        public static Boolean IsInWeek(DateTime date)
        {
            DateTime now = DateTime.Now;
            DateTime week = new DateTime(now.Year, now.Month, now.Day).AddDays(7);
            TimeSpan ts = (date - week);
            
            return (ts.TotalDays < 0);
        }




        public static object getObjectProperty(object o, String property)
        {
            String[] properties = property.Split('.');
            object currentObject = o;

            foreach (String propName in properties)
            {
                if (currentObject != null)
                {
                    String prop = propName;
                    String key = null;

                    // Cas des Key dans une Hash
                    if (prop.IndexOf("@") != -1)
                    {
                        key = prop.Substring(prop.IndexOf("@") + 1);
                        prop = prop.Substring(0, prop.IndexOf("@"));
                    }

                    // Reprendre la property
                    Type type = currentObject.GetType();
                    PropertyInfo propInfo = type.GetProperty(prop);
                    currentObject = propInfo.GetValue(currentObject, null);

                    if (!String.IsNullOrEmpty(key) && currentObject != null && currentObject is IDictionary)
                    {
                        currentObject = ((IDictionary)currentObject)[key];
                    }
                }
            }

            return currentObject;
        }




        public static void GenerateIncFile(int count, String templateFile, String outputFile)
        {
            String template = File.ReadAllText(templateFile);
            
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i <= count; i++)
            {
                sb.Append(String.Format(template, i));
            }

            File.WriteAllText(outputFile, sb.ToString());
        }
    }
}

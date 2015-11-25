using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml.XPath;
using System.Xml;

namespace GCal
{
    public class GCalAPI
    {
        public static String CLIENT_LOGIN_URL = "https://www.google.com/accounts/ClientLogin";
	
	    public static String[] GDATA_VERSION_HEADER = {"GData-Version", "2"};
	
	
	    public static int STAT_ADDED = 0;
	    public static int STAT_UPDATED = 1;
	    public static int STAT_DELETED = 2;


        private static long DEFAULT_MAX_RESULTS = 500;
	
	
	    private String authToken = null;
	    private String[] getAuthHeader()
	    {
		    return new String[] {"Authorization", "GoogleLogin auth=" + authToken};
	    }


        /**
	     * Effectue un login et positionne le authToken pour les différents appels.
	     * 
	     * @param user
	     * @param pwd
	     * @throws Exception
	     */
	    public void Login(String user, String pwd)
	    {
		    authToken = null;
		
		    // Connection vers Google
            StringBuilder content = new StringBuilder();

            content.Append("Email=").Append(user);
            content.Append("&Passwd=").Append(pwd);
            content.Append("&service=").Append("cl");
            content.Append("&source=").Append("GCalApi-0.1");

            String response = Tools.PostPageContent(CLIENT_LOGIN_URL,
                "application/x-www-form-urlencoded", null,
                Tools.GetBytes(content));

            
		
		    if (response != null)
		    {
			    /*
			     * Format de réponse : 
			     * SID=DQAAAGgA...7Zg8CTN
			     * LSID=DQAAAGsA...lk8BBbG
			     * Auth=DQAAAGgA...dk3fA5N
			     */
			
			    if (response.IndexOf("Auth=") != -1)
			    {
                    String auth = response.Substring(response.IndexOf("Auth=") + 5);

                    if (auth.IndexOf("\r") != -1)
                        auth = auth.Substring(0, auth.IndexOf("\r"));

                    if (auth.IndexOf("\n") != -1)
                        auth = auth.Substring(0, auth.IndexOf("\n"));
				
				    authToken = auth;
			    }
		    }
	    }
	
	
	    /**
	     * Se deconnecte des services Google.
	     */
	    public void Disconnect()
	    {
		    authToken = null;
	    }




        /**
	     * Récupère la liste des calendriers de l'utilisateur.
	     * Nécessite un login avant.
	     * 
	     * @return
	     */
        public List<GCalendar> GetCalendars()
        {
            List<GCalendar> calendars = new List<GCalendar>();

            try
            {
                String feedUrl = "https://www.google.com/calendar/feeds/default/allcalendars/full";

                String response = Tools.GetPageContent(feedUrl, null,
                        new String[][] { getAuthHeader(), GDATA_VERSION_HEADER });
                
                calendars = ParseGCalendars(response);
            }
            catch (Exception e)
            {
                throw e;
            }

            return calendars;
        }



        /**
	     * Récupère la liste des calendriers filtrée par leur ID.
	     * 
	     * @param calIds
	     * @return
	     */
        public List<GCalendar> GetCalendars(params String[] calIds)
        {
            List<GCalendar> allCalendars = GetCalendars();
            List<GCalendar> calendars = new List<GCalendar>();

            foreach (GCalendar cal in allCalendars)
            {
                foreach (String calId in calIds)
                {
                    if (calId.Equals(cal.Id))
                        calendars.Add(cal);
                }
            }

            return calendars;
        }


        private List<GCalendar> ParseGCalendars(String response)
        {
            List<GCalendar> calendars = new List<GCalendar>();

            // Parser la réponse en Xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            XmlNamespaceManager x = new XmlNamespaceManager(doc.NameTable);
            x.AddNamespace("x", doc.DocumentElement.NamespaceURI);

            foreach (XmlNode node in doc.SelectNodes("//x:feed/x:entry", x))
            {
                GCalendar gcal = new GCalendar();

                gcal.Id = node.SelectSingleNode("x:id", x).InnerText;
                gcal.Title = node.SelectSingleNode("x:title", x).InnerText;
                gcal.Link = node.SelectSingleNode("x:content/@src", x).InnerText;
                
                calendars.Add(gcal);
            }

            return calendars;
        }





        /**
	     * Récupère la liste des evenements d'un calendrier.
	     * 
	     * @param calendar
	     * @param startMin
	     * @param updatedMin
	     * @param maxResults
	     * @return
	     */
        public List<GEvent> GetEvents(String calUrl, DateTime? startMin, DateTime? updatedMin, long maxResults, Boolean secure)
        {
            List<GEvent> events = new List<GEvent>();
            String response = null;

            if (maxResults < 0)
                maxResults = DEFAULT_MAX_RESULTS;

            String feedUrl = calUrl + "?max-results=" + maxResults;

            try
            {
                // TODO gestion du critere updated-min
                if (updatedMin != null)
                {
                    feedUrl += "&updated-min=" + Uri.EscapeDataString(Tools.GetDateRFC3339(updatedMin));
                }
                else
                {
                    if (startMin != null)
                    {
                        feedUrl += "&start-min=" + Uri.EscapeDataString(Tools.GetDateRFC3339(startMin));
                    }
                }

                // start-max sur les 10 prochaines années
                DateTime max = DateTime.Now.AddYears(10);
                feedUrl += "&start-max=" + Uri.EscapeDataString(Tools.GetDateRFC3339(max));

                feedUrl += "&orderby=starttime";
                feedUrl += "&singleevents=true";
                feedUrl += "&showdeleted=false";
                feedUrl += "&sortorder=a";

                String[][] headers = (secure ? new String[][] { getAuthHeader(), GDATA_VERSION_HEADER } : null);
                response = Tools.GetPageContent(feedUrl, null, headers);

                events = ParseGEvents(response);
            }
            catch (Exception e)
            {
                throw e;
            }

            return events;
        }



        private List<GEvent> ParseGEvents(String response)
        {
            List<GEvent> events = new List<GEvent>();

            // Parser la réponse en Xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            XmlNamespaceManager x = new XmlNamespaceManager(doc.NameTable);
            x.AddNamespace("x", doc.DocumentElement.NamespaceURI);
            x.AddNamespace("gd", "http://schemas.google.com/g/2005");
            x.AddNamespace("gCal", "http://schemas.google.com/gCal/2005");

            GCalendar gc = new GCalendar();
            gc.Id = GetNodeText(doc, "//x:feed/x:id", x);
            gc.Title = GetNodeText(doc, "//x:feed/x:title", x);
            gc.Link = GetNodeText(doc, "//x:feed/x:link[@rel='alternate']/@href", x);

            foreach (XmlNode node in doc.SelectNodes("//x:feed/x:entry", x))
            {
                GEvent item = new GEvent();

                item.Id = GetNodeText(doc, "x:id", x);
                item.Title = GetNodeText(doc, "x:title", x);
                item.Start = Tools.GetDateRFC3339(GetNodeText(doc, "gd:when/@startTime", x));
                item.End = Tools.GetDateRFC3339(GetNodeText(doc, "gd:when/@endTime", x));

                item.Link = GetNodeText(doc, "x:link[@rel='alternate']/@href", x);
                item.IsRecurring = (GetNodeText(doc, "gd:originalEvent", x) != null);

                // All day event if startTime is yyyy-MM-dd (== 10 chars)
                String startTime = GetNodeText(doc, "gd:when/@startTime", x);
                item.IsAllDay = (startTime.Length == 10);
                
                item.Calendar = gc;

                events.Add(item);
            }

            return events;
        }


        private String GetNodeText(XmlDocument doc, String xpath, XmlNamespaceManager x)
        {
            try { return doc.SelectSingleNode(xpath, x).InnerText; }
            catch (Exception) { return ""; }
        }
    }
}

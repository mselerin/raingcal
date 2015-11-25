using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml.XPath;
using System.Xml;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using System.Globalization;

namespace GCal
{
    public class GCalAPIv3
    {
        private String clientId;
        private String clientSecret;
        private String appName;
        private String userName;

        

        public static int STAT_ADDED = 0;
        public static int STAT_UPDATED = 1;
        public static int STAT_DELETED = 2;


        private static int DEFAULT_MAX_RESULTS = 500;



        public GCalAPIv3(String clientId, String clientSecret, String appName, String userName)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.appName = appName;
            this.userName = userName;
        }


        private CalendarService GetCalendarService()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                new[] { CalendarService.Scope.Calendar }, userName, CancellationToken.None
            ).Result;


            CalendarService service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });


            return service;
        }


        private GCalendar Convert(CalendarListEntry c)
        {
            if (c == null)
                return null;

            GCalendar gcal = new GCalendar();

            gcal.Id = c.Id;
            gcal.Title = c.Summary;
            gcal.Link = "";

            return gcal;
        }


        private GEvent Convert(Event e)
        {
            if (e == null)
                return null;

            GEvent item = new GEvent();

            item.Id = e.Id;
            item.Title = e.Summary;

            if (e.Start.DateTime != null)
            {
                item.IsAllDay = false;
                item.Start = e.Start.DateTime.Value;
                item.End = e.End.DateTime.Value;
            }
            else
            {
                item.IsAllDay = true;

                // All day event if startTime is yyyy-MM-dd
                item.Start = DateTime.ParseExact(e.Start.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                item.End = DateTime.ParseExact(e.End.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            item.Link = e.HtmlLink;
            item.IsRecurring = (!String.IsNullOrEmpty(e.RecurringEventId));

            return item;
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

            CalendarService service = GetCalendarService();
            CalendarList list = service.CalendarList.List().Execute();
            foreach (CalendarListEntry o in list.Items)
            {
                calendars.Add(Convert(o));
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


        public GCalendar GetCalendar(String calId)
        {
            List<GCalendar> allCalendars = GetCalendars(calId);
            
            if (allCalendars != null && allCalendars.Count > 0)
                return allCalendars[0];

            return null;
        }






        /**
         * Récupère la liste des evenements d'un calendrier.
         * 
         * @param calId
         * @param startMin
         * @param updatedMin
         * @param maxResults
         * @return
         */
        public List<GEvent> GetEvents(String calId, DateTime? startMin, DateTime? updatedMin, int maxResults)
        {
            List<GEvent> events = new List<GEvent>();

            try
            {
                // Retrouver le calendrier
                GCalendar gcal = GetCalendar(calId);
                
                if (maxResults < 0)
                    maxResults = DEFAULT_MAX_RESULTS;

                EventsResource.ListRequest request = GetCalendarService().Events.List(calId);
                request.MaxResults = maxResults;

                
                // TODO gestion du critere updated-min
                if (updatedMin != null)
                {
                    request.UpdatedMin = updatedMin;
                }
                else if (startMin != null)
                {
                    request.TimeMin = startMin;
                }

                // start-max sur les 10 prochaines années
                DateTime max = DateTime.Now.AddYears(10);
                request.TimeMax = max;

                request.OrderBy = Google.Apis.Calendar.v3.EventsResource.ListRequest.OrderByEnum.StartTime;
                request.SingleEvents = true;
                request.ShowDeleted = false;

                Events gevents = request.Execute();

                foreach (Event e in gevents.Items)
                {
                    GEvent g = Convert(e);
                    g.Calendar = gcal;
                    
                    events.Add(g);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return events;
        }
    }
}

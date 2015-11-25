using System;
using System.Collections.Generic;
using System.Text;
using GCal;

namespace GCalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] calUrls = new String[] {
                "michel.selerin@gmail.com"

            };
            

            int max = 15;

            String clientId = "";
            String clientSecret = "";
            String appName = "";
            String userName = "";

            List<GEvent> events = new List<GEvent>();
            GCalAPIv3 gcal = new GCalAPIv3(clientId, clientSecret, appName, userName);

            for (int i = 0; i < calUrls.Length; i++)
            {
                String calUrl = calUrls[i];

                //String baseUrl = "https://www.google.com/calendar/feeds/" + calUrl + "/basic";
                List<GEvent> e = gcal.GetEvents(calUrl, DateTime.Now, null, max);

                // On met le CalId dans les Data (pour le bloc de couleur)
                foreach (GEvent ge in e)
                {
                    ge.Data["CalId"] = i.ToString();
                }

                events.AddRange(e);
            }

            // Trier la liste
            events.Sort();

            if (events.Count > max)
            {
                events.RemoveRange(max, (events.Count - max));
            }

            foreach (GEvent ge in events)
            {
                Console.WriteLine(" > " + Tools.getObjectProperty(ge, "Title") + " - " + Tools.getObjectProperty(ge, "Start") + " - " + ge.IsAllDay);
            }


            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}

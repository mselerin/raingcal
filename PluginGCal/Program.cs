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
                "en.uk%23holiday%40group.v.calendar.google.com/public"
            };

            int max = 15;

            List<GEvent> events = new List<GEvent>();
            GCalAPI gcal = new GCalAPI();

            for (int i = 0; i < calUrls.Length; i++)
            {
                String calUrl = calUrls[i];

                String baseUrl = "http://www.google.com/calendar/feeds/" + calUrl + "/full";
                List<GEvent> e = gcal.GetEvents(baseUrl, DateTime.Now, null, max, false);

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
                Console.WriteLine(" > " + Tools.getObjectProperty(ge, "Title") + " - " + Tools.getObjectProperty(ge, "Start") + " - " + Tools.IsInWeek(ge.Start));
            }


            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}

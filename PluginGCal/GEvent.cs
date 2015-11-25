using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GCal
{
    public class GEvent : IComparable<GEvent>
    {
        private String id;

        public String Id
        {
            get { return id; }
            set { id = value; }
        }
        private String title;

        public String Title
        {
            get { return title; }
            set { title = value; }
        }
        private String link;

        public String Link
        {
            get { return link; }
            set { link = value; }
        }

        private DateTime start;

        public DateTime Start
        {
            get { return start; }
            set { start = value; }
        }
        private DateTime end;

        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }

        private Boolean isAllDay = false;

        public Boolean IsAllDay
        {
            get { return isAllDay; }
            set { isAllDay = value; }
        }

        private Boolean isRecurring = false;

        public Boolean IsRecurring
        {
            get { return isRecurring; }
            set { isRecurring = value; }
        }

        private Dictionary<String, String> data = new Dictionary<String, String>();

        public Dictionary<String, String> Data
        {
            get { return data; }
            set { data = value; }
        }


        private GCalendar calendar;

        public GCalendar Calendar
        {
            get { return calendar; }
            set { calendar = value; }
        }


        public int CompareTo(GEvent other)
        {
            int c = this.Start.CompareTo(other.Start);

            if (c == 0)
                c = this.End.CompareTo(other.End);

            if (c == 0)
                c = this.Title.CompareTo(other.Title);

            return c;
        }
    }
}

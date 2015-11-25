using System;
using System.Collections.Generic;
using System.Text;

namespace GCal
{
    public class GCalendar
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

        public override String ToString()
        {
            return this.Title;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;
using GCal;
using System.Text;
using System.IO;


namespace PluginGCal
{
    internal class Measure
    {
        internal virtual void Reload(Rainmeter.API api, ref double maxValue)
        {
            
        }

        internal virtual double Update()
        {
            return 0.0;
        }

        internal virtual String GetString()
        {
            return "";
        }
    }


    internal class ParentMeasure : Measure
    {
        public static string PARAM_CAL = "Cal";
        public static string PARAM_MAX = "Max";
        public static string PARAM_TODAY_FORMAT = "TodayDateFormat";
        public static string PARAM_WEEK_DATE_FORMAT = "WeekDateFormat";
        public static string PARAM_DATE_FORMAT = "DateFormat";
        public static string PARAM_INC_PATH = "IncludePath";

        public static string PARAM_GOOGLE_CLIENT_ID = "GoogleClientId";
        public static string PARAM_GOOGLE_CLIENT_SECRET = "GoogleClientSecret";
        public static string PARAM_GOOGLE_APP_NAME = "GoogleAppName";
        public static string PARAM_GOOGLE_USER_NAME = "GoogleUserName";
        

        internal string Name;
        internal IntPtr Skin;
        
        static int DEFAULT_MAX = 8;
        static long MIN_INTERVAL = 30000L;

        internal string urlFormat = "https://www.google.com/calendar/feeds/{0}/full";
        internal List<string> calUrls = new List<string>();
        internal int max = DEFAULT_MAX;

        internal long lastUpdate = 0;
        internal List<GEvent> lastEvents = new List<GEvent>();
        internal String weekDateFormat;
        internal String dateFormat;
        internal String todayDateFormat;

        internal String googleClientId;
        internal String googleClientSecret;
        internal String googleAppName;
        internal String googleUserName;

        internal ParentMeasure()
        {
        }

        internal override void Reload(Rainmeter.API api, ref double maxValue)
        {
            base.Reload(api, ref maxValue);

            Name = api.GetMeasureName();
            Skin = api.GetSkin();
            
            calUrls.Clear();

            int i = 1;
            String tmp = null;

            while (!String.IsNullOrEmpty((tmp = api.ReadString(PARAM_CAL + i, ""))))
            {
                calUrls.Add(tmp);
                i++;
            }

            max = api.ReadInt(PARAM_MAX, DEFAULT_MAX);

            todayDateFormat = api.ReadString(PARAM_TODAY_FORMAT, "HH:mm");
            weekDateFormat = api.ReadString(PARAM_WEEK_DATE_FORMAT, "ddd");
            dateFormat = api.ReadString(PARAM_DATE_FORMAT, "ddd dd MMM");

            googleClientId = api.ReadString(PARAM_GOOGLE_CLIENT_ID, "");
            googleClientSecret = api.ReadString(PARAM_GOOGLE_CLIENT_SECRET, "");
            googleAppName = api.ReadString(PARAM_GOOGLE_APP_NAME, "");
            googleUserName = api.ReadString(PARAM_GOOGLE_USER_NAME, "");

            API.Log(API.LogType.Notice, "GCal.dll : googleAppName : " + googleAppName);

            API.Log(API.LogType.Notice, "GCal.dll: Calendars : " + calUrls.Count + " - Max results : " + max);

            
            // Generates include files
            String path = api.ReadPath(PARAM_INC_PATH, "");
            if (!String.IsNullOrEmpty(path))
            {
                Tools.GenerateIncFile(max
                        , Path.Combine(path, "TMeasures.inc")
                        , Path.Combine(path, "Measures.inc"));

                Tools.GenerateIncFile(max
                    , Path.Combine(path, "TMeters.inc")
                    , Path.Combine(path, "Meters.inc"));
            }

            lastUpdate = 0; // Forcer la mise à jour
            UpdateEvents();
        }

        internal override double Update()
        {
            UpdateEvents();
            return lastEvents.Count;
        }


        internal Boolean UpdateEvents()
        {
            try
            {
                long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                // Tester si la mesure n'a pas été faite trop recemment
                if (now - lastUpdate < MIN_INTERVAL)
                {
                    API.Log(API.LogType.Notice, "GCal.dll: Update interval too short !");
                    return false;
                }

                List<GEvent> events = new List<GEvent>();
                GCalAPIv3 gcal = new GCalAPIv3(googleClientId, googleClientSecret, googleAppName, googleUserName);

                API.Log(API.LogType.Notice, "GCal.dll: About to read events...");

                for (int i = 0; i < calUrls.Count; i++)
                {
                    String calUrl = calUrls[i];
                    API.Log(API.LogType.Notice, "GCal.dll: Retrieving calendar [" + calUrl + "]");

                    List<GEvent> e = gcal.GetEvents(calUrl, DateTime.Now, null, max);

                    // On met le CalId dans les Data (pour le bloc de couleur)
                    foreach (GEvent ge in e)
                    {
                        ge.Data.Add("CalId", (i + 1).ToString());
                    }

                    events.AddRange(e);
                }

                // Trier la liste
                events.Sort();

                // Ne garder que les <max> premiers résultats
                if (events.Count > max)
                {
                    events.RemoveRange(max, (events.Count - max));
                }

                lastUpdate = now;
                lastEvents = events;

                return true;
            }
            catch (Exception e)
            {
                API.Log(API.LogType.Error, "GCal.dll: Cannot update events : " + e.Message);
            }
            
            return false;
        }


        internal override String GetString()
        {
            return "";
        }
    }






    internal class ChildMeasure : Measure
    {
        public static String PARAM_PARENT_NAME = "ParentName";
        public static String PARAM_PROPERTY = "Property";
        public static String PARAM_INDEX = "Index";
        
        
        private bool HasParent = false;
        private uint ParentID;
        private int ndx;
        private String property;
        internal String todayDateFormat;
        internal String weekDateFormat;
        internal String dateFormat;
        

        internal override void Reload(Rainmeter.API api, ref double maxValue)
        {
            base.Reload(api, ref maxValue);

            string parentName = api.ReadString(PARAM_PARENT_NAME, "");
            IntPtr skin = api.GetSkin();

            property = api.ReadString(PARAM_PROPERTY, "");
            

            String mesureName = api.GetMeasureName();
            if (mesureName.StartsWith(parentName))
            {
                String sndx = mesureName.Substring(parentName.Length);
                if (sndx.IndexOf("_") > 0)
                {
                    if (String.IsNullOrEmpty(property))
                        property = sndx.Substring(sndx.IndexOf("_") + 1);

                    sndx = sndx.Substring(0, sndx.IndexOf("_"));
                }

                ndx = int.Parse(sndx);
            }
            else
            {
                ndx = api.ReadInt(PARAM_INDEX, 1);
            }

            ndx--;


            // Find parent using name AND the skin handle to be sure that it's the right one
            RuntimeTypeHandle parentType = typeof(ParentMeasure).TypeHandle;
            foreach (KeyValuePair<uint, Measure> pair in Plugin.Measures)
            {
                if (System.Type.GetTypeHandle(pair.Value).Equals(parentType))
                {
                    ParentMeasure parentMeasure = (ParentMeasure)pair.Value;
                    if (parentMeasure.Name.Equals(parentName) &&
                        parentMeasure.Skin.Equals(skin))
                    {
                        HasParent = true;
                        ParentID = pair.Key;

                        todayDateFormat = api.ReadString(ParentMeasure.PARAM_TODAY_FORMAT, parentMeasure.todayDateFormat);
                        weekDateFormat = api.ReadString(ParentMeasure.PARAM_WEEK_DATE_FORMAT, parentMeasure.weekDateFormat);
                        dateFormat = api.ReadString(ParentMeasure.PARAM_DATE_FORMAT, parentMeasure.dateFormat);

                        return;
                    }
                }
            }

            HasParent = false;
            API.Log(API.LogType.Error, "GCal.dll: ParentName=" + parentName + " not valid");
        }


        internal override double Update()
        {
            // Retourner 1 ou 0 si la valeur est boolean. Sinon -1.
            Object o = GetValue();
            if (o == null) return -1;

            if (o is Boolean)
            {
                Boolean b = (Boolean) o;
                return (b ? 1 : 0);
            }

            return 0.0;
        }


        internal override String GetString()
        {
            Object o = GetValue();
            return (o != null ? o.ToString() : null);
        }


        internal Object GetValue()
        {
            if (HasParent)
            {
                ParentMeasure parent = (ParentMeasure)Plugin.Measures[ParentID];

                if (parent.lastEvents.Count > ndx)
                {
                    try
                    {
                        GEvent ge = parent.lastEvents[ndx];
                        object value = Tools.getObjectProperty(ge, property);

                        API.Log(API.LogType.Debug, "GCal.dll: Value with index [" + ndx + "] and property [" + property + "] : " + value);

                        if (value == null)
                        {
                            return "";
                        }

                        if (value is DateTime)
                        {
                            DateTime dt = (DateTime)value;

                            if (Tools.IsToday(dt))
                                return dt.ToString(todayDateFormat);
                            
                            else if (Tools.IsInWeek(dt))
                                return dt.ToString(weekDateFormat);
                            
                            else
                                return dt.ToString(dateFormat);
                        }

                        else if (value is Boolean)
                        {
                            return ((Boolean) value) ? "1" : "0"; // See Update
                        }

                        else
                        {
                            return value.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        API.Log(API.LogType.Error, "GCal.dll: Exception : " + e.Message);
                    }
                }
            }

            API.Log(API.LogType.Warning, "GCal.dll: Cannot get value with index [" + ndx + "] and property [" + property + "]");
            return null;
        }
    }

    

    public static class Plugin
    {
        internal static Dictionary<uint, Measure> Measures = new Dictionary<uint, Measure>();

        [DllExport]
        public unsafe static void Initialize(void** data, void* rm)
        {
            uint id = (uint)((void*)*data);
            Rainmeter.API api = new Rainmeter.API((IntPtr)rm);

            string parent = api.ReadString("ParentName", "");
            if (String.IsNullOrEmpty(parent))
            {
                Measures.Add(id, new ParentMeasure());
            }
            else
            {
                Measures.Add(id, new ChildMeasure());
            }
        }

        [DllExport]
        public unsafe static void Finalize(void* data)
        {
            uint id = (uint)data;
            Measures.Remove(id);
        }

        [DllExport]
        public unsafe static void Reload(void* data, void* rm, double* maxValue)
        {
            uint id = (uint)data;
            Measures[id].Reload(new Rainmeter.API((IntPtr)rm), ref *maxValue);
        }

        [DllExport]
        public unsafe static double Update(void* data)
        {
            uint id = (uint)data;
            return Measures[id].Update();
        }

        [DllExport]
        public unsafe static char* GetString(void* data)
        {
            uint id = (uint)data;
            fixed (char* s = Measures[id].GetString()) return s;
        }
    }
}

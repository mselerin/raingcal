raingcal
========

Google Calendar plugin for Rainmeter

This plugin was inspired by the GCal skins from Gnometer.

Features :

- Support for multiple calendars (potentially an infinite numbers). All events are merged and sorted by start date.
- Configurable date format
- Specific date format for today's events and week's events (for example, display 12:00 for today or 'sat.' for next saturday events)
- Automatically generates include files for measures and meters according to "Max" parameters (need to Update twice the skin)
- Retrieve measure according to the measure name or by the "Property" parameter


Usage :

- Add the rainmeter SDK 'API' folder in the solution root folder (https://github.com/rainmeter/rainmeter-plugin-sdk/tree/master/API)
- Compile and put GCal.dll on [RAINMETER]/Plugins folder
- Checkout the "PGcal" folder for a fully functionnal skin
- Extract the XML private url from Google Calendar (see the settings for a calendar) and get that part : https://www.google.com/calendar/feeds/[CALENDAR_ID]/basic



Exposed Fields :

- Id (String) : event ID
- Title (String) : event title
- Link (String) : event link (details page)
- Start (DateTime) : start date
- End (DateTime) : end date
- IsRecurring (Boolean) : is it a recurring event or not ?
- IsAllDay (Boolean) : is it an all day event or not ?
- Calendar.Id (String) : event's calendar ID
- Calendar.Title (String) : event's calendar title
- Calendar.Link (String) : event's calendar link

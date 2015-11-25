[Rainmeter]
Update=300000
BackgroundMode=2
DynamicWindowSize=1
SolidColor=00000050


[Metadata]
Name=Google Calendar
Description=Shows upcoming events in Google Calendar.
Instructions=Refer to the manual to display your Gcal events
Version=1.3.0
License=Creative Commons BY-NC-SA 3.0

[Variables]
@IncludeVariables=Variables.inc


; MEASURES ====================================

[mGcal]
Measure=Plugin
Plugin=GCal.dll
Cal1=#mGcal.Cal1#
Cal2=#mGcal.Cal2#
Cal3=#mGcal.Cal3#
TodayDateFormat=#format.Time#
WeekDateFormat=#format.Week#
DateFormat=#format.Date#
Max=#mGcal.Max#
IncludePath=#CURRENTPATH#
GoogleClientId=#mGcal.GoogleClientId#
GoogleClientSecret=#mGcal.GoogleClientSecret#
GoogleAppName=#mGcal.GoogleAppName#
GoogleUserName=#mGcal.GoogleUserName#


@IncludeMeasures=Measures.inc



; STYLES ======================================

@IncludeStyle=Style.inc


; METERS ======================================

[Title]
Meter=STRING
MeterStyle=sTitle
Text="Google Calendars"
ToolTipText="Open Google Calendar"
LeftMouseUpAction=!Execute [http://www.google.com/calendar]
MouseActionCursor=1

[Skin]
Meter=IMAGE
MeterStyle=sSkin


; METERS ITEMS ============================

@IncludeMeters=Meters.inc

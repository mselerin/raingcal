; ===================== MEASURES {0} =====================

[mGcal{0}_IsAllDayTDF]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal
Property=IsAllDay
Substitute="1":"'all day'","0":"HH:mm"

[mGcal{0}_IsAllDayWDF]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal
Property=IsAllDay
Substitute="1":"ddd 'all day'","0":"ddd HH:mm"

[mGcal{0}_Title]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal

[mGcal{0}_Start]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal
Property=Start
TodayDateFormat=[mGcal{0}_IsAllDayTDF]
WeekDateFormat=[mGcal{0}_IsAllDayWDF]
DynamicVariables=1

[mGcal{0}_Data@CalId]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal

[mGcal{0}_Calendar.Link]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal

[mGcal{0}_Link]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal

[mGcal{0}_StartTooltip]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal
Property=Start
DateFormat=ddd dd MMMMM HH:mm

[mGcal{0}_EndTooltip]
Measure=Plugin
Plugin=GCal.dll
ParentName=mGcal
Property=End
DateFormat=ddd dd MMMMM HH:mm



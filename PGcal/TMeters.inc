; ===================== METERS {0} =====================

[Item{0}Bloc]
Meter=Image
MeasureName=mGcal{0}_Data@CalId
ImageName=cal%1.png
MeterStyle=sColorBloc
LeftMouseUpAction=!Execute ["[mGcal{0}_Calendar.Link]"]
MouseActionCursor=1

[Item{0}Left]
Meter=String
MeterStyle=sTextL
MeasureName=mGcal{0}_Title
LeftMouseUpAction=!Execute ["[mGcal{0}_Link]"]
MouseActionCursor=1

[Item{0}Right]
Meter=String
MeterStyle=sTextR
MeasureName=mGcal{0}_Start
MeasureName2=mGcal{0}_StartTooltip
MeasureName3=mGcal{0}_EndTooltip
ToolTipText=%2#CRLF#%3

[Item{0}Bar]
Meter=Image
MeterStyle=sSeperator


[Variables]
font.Name=Segoe UI
font.Size=7
font.Style=SHADOW
skin.ToolTipHidden=0
skin.Width=250
skin.Height=((#h.Item# * #mGcal.Max#) + 10)
h.Item=19
color.Skin=00000033
color.Title=FFFFFFFF
color.Text=FFFFFFFF
color.Seperator=FFFFFF33

h.Other=(#h.Item#-7)
w.TextR=90
w.TextL=(#skin.Width#-#w.TextR#)
w.ToolTip=(#skin.Width# + 100)


[sSkin]
X=0
Y=28
W=#skin.Width#
H=#skin.Height#
SolidColor=#color.Skin#

[sTitle]
X=10
Y=7
W=(#skin.Width#-40)
H=14
FontColor=#color.Title#
FontFace=#font.Name#
FontSize=(#font.Size# + 1)
StringCase=NONE
StringStyle=BOLD
StringEffect=#font.Style#
StringAlign=LEFT
ClipString=1
AntiAlias=1


[sColorBloc]
X=5
Y=5r
ClipString=1
AntiAlias=1
Group=PGcalItem


[sTextL]
X=20
Y=0r
H=#h.Other#
W=#w.TextL#
ClipString=1
FontColor=#color.Text#
FontFace=#font.Name#
FontSize=#font.Size#
StringCase=NONE
StringStyle=BOLD
StringEffect=#font.Style#
FontEffectColor=00000040
StringAlign=LEFT
AntiAlias=1
ToolTipType=1
ToolTipWidth=#w.ToolTip#
ToolTipHidden=#skin.ToolTipHidden#
ToolTipText=%1
Group=PGcalItem


[sTextR]
X=(#skin.Width#-5)
Y=0r
H=#h.Other#
W=#w.TextR#
ClipString=1
FontColor=#color.Text#
FontFace=#font.Name#
FontSize=#font.Size#
StringCase=NONE
StringStyle=BOLD
StringEffect=#font.Style#
FontEffectColor=00000040
StringAlign=RIGHT
AntiAlias=1
ToolTipType=1
ToolTipWidth=#w.ToolTip#
ToolTipHidden=#skin.ToolTipHidden#
Group=PGcalItem


[sSeperator]
X=6
Y=(#h.Other#+2)r
W=(#skin.Width#-12)
H=1
SolidColor=#color.Seperator#
BarOrientation=HORIZONTAL
Group=PGcalItem

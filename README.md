# TrackRename
Track renaming plugin for MusicBee

This program is a plugin for MusicBee.
This program is written in C# language.
This plug-in renames the track names of songs imported with MusicBee using a separately prepared text file.
The character code of the text file used for renaming is prepared in Shift-JIS (MS932).
Create a text file by separating the name of the track to be renamed and the name to be renamed on one line with a comma or tab character.
To use it, select the target track on the album display screen in MusicBee and select the plug-in menu from the context menu (right-click).
There are two plugin menus. Rename the track names by selecting comma delimiter or tab delimiter depending on the renaming text file you have prepared.
The renaming results are saved as a log file in MusicBee's AppData.
Log files are saved up to 30 days ago. Older log files will be deleted accordingly.

# CheckProcess  
  
Deutscher Text unter dem englischen Text. / German text under the English one.  
  
-----------------------------------------
My first plugin for Touch Portal.  
I didn't find anything in this direction and I couldn't use my script flexible enough.  
  
I use the following project in this project:  
https://github.com/mpaperno/TouchPortal-CS-API  
At the same time to the naming a thanks for creation, as well as publication.  
  
It checks if a process / application is running and returns the state "running" or "not running" accordingly.  
So the state can be used as an if-condition.  
For example, I use it together with a Spotify plugin for the play/pause button. If Spotify is not running, it is started, then waits 3 seconds for the start and only then "Play" is executed.  
The built-in timer also allows you to react to a status change. For example, adjust the button colors for Spotify, according to whether Spotify is running or not.  
A screenshot of my Spotify play/pause button below.  
  
The entry.tp can be edited manually to use additional processes. These then become available in the settings afterwards.  
Just copy a block like this inside the file:  
```
    {  
      "name": "Process 2",  
      "type": "text"  
    },
```
There is not much to pay attention to:  
- one comma after each curly bracket / except at the end
- continuous numbering, starting at 1 (otherwise nothing should be adjusted)
  
Settings:  
![image](https://github.com/TripleT3005/TouchPortal_CheckProcess/assets/97241354/64f72b0f-4770-491f-b09d-e61f13748e33)
  
Spotify Play-/Pause-Button:  
![image](https://github.com/TripleT3005/TouchPortal_CheckProcess/assets/97241354/bfebf74e-7bc0-4e57-919f-76e9f73a8b5e)
  
-----------------------------------------
Mein erstes Plugin für Touch Portal.  
Ich habe nichts in dieser Richtung gefunden und mein Skript konnte ich nicht flexibel genug nutzen.  
  
Folgendes Projekt nutze ich in diesem Projekt:  
https://github.com/mpaperno/TouchPortal-CS-API  
Gleichzeitig zur Nennung ein Dank für Erstellung, sowie Veröffentlichung.  
  
Es prüft ob ein Prozess / Anwendung läuft und gibt entsprechend den Status "running" oder "not running" zurück.  
Somit kann der Status als If-Bedingung verwendet werden.  
Ich nutze es beispielsweise zusammen mit einem Spotify-Plugin für den Play-/Pause-Button. Wenn Spotify nicht läuft, wird es gestartet, dann 3 Sekunden auf den Start gewartet und erst dann "Play" ausgeführt.  
Durch den eingebauten Timer kann auch auf eine Statusänderung reagiert werden. Beispielsweise die Button-Farben für Spotify anpassen, entsprechend ob Spotify läuft oder nicht läuft.  
Ein Screenshot meines Spotify Play-/Pause-Buttons weiter unten.  
  
Die entry.tp kann händisch editiert werden, um weitere Prozesse nutzen zu können. Diese werden dann anschließend in den Einstellungen verfügbar.  
Einfach einen solchen Block innerhalb der Datei kopieren:  
```
    {  
      "name": "Process 2",  
      "type": "text"  
    },
```
Dabei muss nicht auf viel geachtet werden:  
- jeweils ein Komma nach der geschweiften Klammer / außer am Ende
- fortlaufende Nummerierung, beginnend bei 1 (ansonsten sollte nichts angepasst werden)
  
Einstellungen:  
![image](https://github.com/TripleT3005/TouchPortal_CheckProcess/assets/97241354/8d85638c-7780-4c8d-b9c5-bfee40810c2b)
  
Spotify Play-/Pause-Button:  
![image](https://github.com/TripleT3005/TouchPortal_CheckProcess/assets/97241354/cf76b1d0-fec0-4b73-a99f-683640b0cd1f)

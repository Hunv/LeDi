Willkommen zur LeDi's Dokumentation!
=====================================

**LeDi** ist ein Open Source Software und Hardware Projekt, welches dazu dient Spiele, vornehmlich sportlicher Natur, mit einer Anzeigetafel zu begleiten und gleichzeitig das Spielmanagement an sich zu übernehmen (Zeitmessung, Spielerverwaltung uvm.).


Projekt auf GitHub: `LeDi <https://github.com/Hunv/LeDi>`_

Das Projekt unterteilt sich in fünf Unterprojekte:

LeDi.Docs
##########
Dieses Projekt stellt die Dokumentation des Projektes da. Hier ist alles zu finden was benötigt wird, um das Projekt nachzubauen und selbst zu betreiben sowie Überlegungen und Hintergründe. Diese Seite ist der Hauptbestandteil dieses Projekts.
Auch die Installation und Konfiguration eines Raspberry Pis zum Betrieb der Softwarekomponenten ist Teil dieses Projektes

LeDi.Server
############
Dies ist die Softwarekomponente, welche die Spielsteuerung übernimmt sowie eine API bereitstellt um den anderen Softwarekomponenten via REST API eine Datenbasis bereit zu stellen.
Nur LeDi.Server hält Daten permenent vor und ist das führende und zentrale Glied in der Kette.
Die Entwicklung geschieht in C# auf Basis von .Net 6.0.

LeDi.WebClient
###############
Dies ist die Softwarekomponente, welche den Schiedsrichtern und anderen Interessierten eine Oberfläche bietet, mit der die zentrale Komponente gesteuert werden kann. 
Die Entwicklung geschieht in C#, HTML, CSS und JavaScript auf Basis von Microsoft Blazor (Server) und .Net 6.0.

LeDi.Display
#############
Dies ist die Softwarekomponente, welche ein LED Display ansteuert, welches auf Basis von WS2812B LEDs erstellt wurde. Gesteuert wird dies ebenfalls über die Zentrale Softwarekomponente.
Die Entwicklung geschieht in C# auf Basis von .Net 6.0

LeDi.Hardware
##############
Dieses Projekt dokumentiert den Bau der Hardware des LED Displays. Dies ist die Hardwarekomponente von LeDi.Display.
Dieses Projekt beinhaltet sowohl den Bau eines Gehäuses mit den LEDs und Netzteilen sowie die Gestaltung der Elektronik. Letzteres sollte auf jeden Fall von einem Elektriker begleitet werden um Gefahr für Leib und Leben sowohl bei der Konstruktion als auch beim späteren Betrieb ausschließen zu können.



.. note::

   Dieses Projekt und alle Unterprojekte sind noch in Entwicklung

Inhalt
--------

.. toctree::

   ledi-server
   Ledi-webclient
   ledi-display
   ledi-hardware
   ledi-docs
   ledi-raspberrysetupEN

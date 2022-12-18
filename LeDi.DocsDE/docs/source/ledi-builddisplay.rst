LeDi Baubegleitung
==================

Diese Seite ist im Aufbau

Diese Seite soll dabei unterstützen ein LED Display selbst zusammen zu bauen. Dies ist keine Schritt für Schritt Anleitung, aber hier ist dokumentiert wie wir unser Display aufgebaut haben und auch was wir ggf. beim nächsten Mal anders machen würden.
Hilfreich für einen Nachbau ist auch unsere Zusammfassung in folgendem YouTube-Video: 
`Link <https://youtu.be/YVUDnpkR8ug>`_ 

Display
-------

Rahmen
#########
Der Rahmen des Displays besteht im Wesentlichen aus einem Rahmen aus Standard-Aluprofilen mit 3cm Kantenlänge ("3030 Nut8 I-Profil"). Der Rahmen, in den der Träger für das Display eingelassen ist, ist ein Doppelprofilrahmen. Also 3060-Profile mit 3cm bzw. 6cm Kantenlänge. Verbunden sind diese Profile mit Nut8 Winkelverbindern.

LED-Träger
##########
Der Träger für die LED Strips besteht aus einer zurecht geschnittenen Schreibtischstuhlunterlage. Diese ist robust, leicht und günstig. Alternativ ginge auch jede andere Art von Trägermaterial, das nicht leitend ist. Vor den LEDs als Schutz und Diffusor haben wir eine weitere Schreibtischstuhlunterlage verwendet. Diese hat hier durch die "perlige" Oberfläche die Wirkung eines Diffusors. Dadruch wirken die LEDs ein bisschen weniger Pixelig. Wer diesen Effekt noch weiter intensivieren möchte, kann eine grobe Milchglasfolie auf die glatte Seite der Blende kleben. Dies gilt auch, wenn statt eine Schreibtischstuhlunterlage ein einfaches transparentes Plexiglas genommen wird. Auch hier hilft Milchglasfolie. Gegebenenfalls diese sogar auf beiden Seiten der Plexiglasscheibe aufbringen.

LEDs
####
Die LEDs sind WS2812B LED-Strips mit 96 LEDs pro Meter. Es gibt diese auch mit 100 LEDs pro Meter. Die breite eines LED-Strips beträgt ca. 1,2cm. Hierdurch entsteht ein nahezu identischer Abstand zwischen den LEDs vertikal und horizontal, wenn mehrere LED-Strips lebeneinander gelegt werden. Unsere LED-Strips waren IP67 zertifiziert. Da durch die Silikonummantelung der LED-Strips jedoch weitere 2mm Breite der LED-Strips hinzu kommen, haben wir diese Ummantelung entfernt. Es gibt IP65 LED-Strips, welche ebenfalls eine gewisse Resistenz gegen Wasser haben, dabei aber nicht an Breite zunehmen. Diese haben lediglich einen Überzug aus Silikon. Diese Variante empfehle ich für alle, die LeDi mobil-, schwimmbad- oder outdoor-fähig machen möchten. Soll LeDi lediglich in trockenen indoor-Umgebungen eingesetzt werden, kann auf IP-Zertifizierungen auch verzichtet werden. Des Weiteren gibt es die LED-Strips in mehreren Längen zu kaufen. In der Regel bedeutet eine längere Variante, dass diese günstiger pro Meter wird. Es empfiehlt sich allerdings nicht nun die längste Variante zu kaufen und diese zurecht zu schneiden - sofern es die passende Größe für die angestrebte Displaygröße auch von der Stange gibt. Meist haben die gekauften LED-Strips am Anfang und am Ende einen Stecker bereits angelötet. Diese können die spätere Montage erheblich vereinfachen, da so stundenlange Lötarbeiten überflüssig werden.
Die LED-Strips selbst haben wir mit einfachem Sanitär-Silikon auf dem LED-Träger aufgeklebt. Wir haben weißes Silikon genommen, jedoch würde ich ehr transparentes Silikon empfehlen. Wenn ein bisschen Silikon zwischen den LED-Strips hervor quillt, ist transparent weniger auffällig als weiß.
Die LED-Strips haben für die Datenleitung des Weiteren eine Richtung. Diese ist üblicherweise mit einem kleinen Pfeil auf den Strips bei jeder LED markiert. Die Strips müssen nun immer abwechselnd in der Richtung der Daten aufgeklebt werden. D.h. wenn die erste Reihe von rechts nach links verläuft, verläuft die zweite Reihe von links nach rechts. usw.

Verkabelung
###########
Jeder LED-Strip hat 3 Leitungen: Plus, Minus und Daten. Plus und Minus müssen durch den LED-Träger hinter die Träger-Platte geführt werden. Hier bietet es sich an mit einem Bohrer, der einen möglichst passenden Durchmesser zum Kabel besitzt, ein Loch in den Träger zu bohren. So kann das Kabel anschließend von hinten nach vorne geführt werden und dort am LED-Strip festgelötet werden.
96 LEDs unserer WS2812B-LEDs brauchen ca. 2,5A Leistung wenn alle LEDs auf voller Helligkeit in weiß leuchten. Die Netzteile liefern 12A. Mit ein wenig Puffer von 2A kann ein Netzteil also 4 LED-Strips versorgen. Daher werden hinter dem LED-Träger 4 Kabel der LED-Strips zu einem zusammengeführt. Hierzu haben wir 5er-Klemmen benutzt. Ggf. eigenen sich hier auch kurze Reihenklemmen. Da dies für Plus und Minus gemacht werden muss, kommen wir am Ende auf insgesamt 15 Klemmen mit Plus und 15 Klemmen mit Minus. Als Kabel haben wir von den LED-Stips zu den Klemmen 1,0mm²-Leitungen verwendet (entsprechend rot und schwarz). Ab den Klemmen zu den Steckdosen haben wir 1,5mm² verwendet. Dies entsprecht der Dicke der Leitungen im Verbindungskabel.
Eine weitere Beachtung finden muss, dass alle Masse-Kabel untereinander verbunden sein müssen. Wenn die LED-Strips keine gemeinsamen Masse haben, werden diese später nicht funktionieren.

Datenleitung
############
Die Zuleitung der Daten muss am ANFANG der LED-Strips an den ersten LED-Strip bei der ersten LED angelötet werden. Die Datenleitung ist der mittlere Pin. Anschließend müssen die einzelnen LED-Strips untereinander miteinander verbunden werden. Dazu wird dann das Ende des ersten Strips mit dem Anfang des zweiten Strips verbunden. Dazu braucht man ein kurzes, ca. 10cm langes Stück Kabel (bei uns: 1,0mm²), welches quasi einen U-Turn vom ersten zum zweiten Strip macht. Dies wird dann bis zum Ende fortgeführt (Ende von Strip 2 mit Anfang von Strip 3 verbinden, ...).

Stecker
#######
Die Stecker sind Standard-Stecker, welche üblicherweise für das Verbinden von Anhängern mit Autos genutzt wird. Wir haben hier die 13-polige Variante verwendet. Nun haben wir jedoch insgesamt 31 Leitungen, die wir mit dem Schaltkasten verbinden müssen. D.h. selbst zwei Stecker (=26 Leitungen) würden Augenscheinlich nicht reichen. Das stimmt aber nicht ganz. Laut DIN-Norm sind bei den Anhänger-Steckern 9 Leitungen auf 1,5mm² und 4 Leitung mit 2,5mm² ausgelegt. Wenn wir jetzt also 8x 2 Leitungen zu einer zusammenfassen, reduziert sich die Gesamtzahl der Leitungen auf 23. Das passt.
D.h. wir haben jeweils 8 Leitungen von Plus und Minus genommen und zu 4 Leitungen mit 3er-Klemmen zusammengefasst. Hier haben wir ab der 3er-Klemme zum Stecker dann Kabel in der 2,5mm²-Variante verwendet - wie es auch in der Spezifikation der Stecker vorgesehen ist.
Wir haben daher dann die einzelen Pins der Stecker wie folgt belegt:

Stecker 1
+++++++++
(Leitung#: Zahl = Leitungsnummer, +/- = Plus- oder Minus-Pol)

+-----+----------------+------------+-------------+
| Pin | Ader-Farbe     | Ltg.-Dicke | Leitung #   |
+=====+================+============+=============+
| 1   | gelb           | 1,5 mm²    | 5+          |
+-----+----------------+------------+-------------+
| 2   | blau           | 1,5 mm²    | 7+          |
+-----+----------------+------------+-------------+
| 3   | weiß oder pink | 1,5 mm²    | 9+          |
+-----+----------------+------------+-------------+
| 4   | grün           | 1,5 mm²    | 11+         |
+-----+----------------+------------+-------------+
| 5   | braun          | 1,5 mm²    | 5-          |
+-----+----------------+------------+-------------+
| 6   | rot            | 1,5 mm²    | 7-          |
+-----+----------------+------------+-------------+
| 7   | schwarz        | 1,5 mm²    | 9-          |
+-----+----------------+------------+-------------+
| 8   | grau           | 1,5 mm²    | Daten       |
+-----+----------------+------------+-------------+
| 9   | braun/blau     | 2,5 mm²    | 1+ und 3+   |
+-----+----------------+------------+-------------+
| 10  | braun/rot      | 2,5 mm²    | 13+ und 15+ |
+-----+----------------+------------+-------------+
| 11  | weiß/rot       | 2,5 mm²    | 1- und 3-   |
+-----+----------------+------------+-------------+
| 12  | lila           | 1,5 mm²    | 11-         |
+-----+----------------+------------+-------------+
| 13  | weiß/schwarz   | 2,5 mm²    | 13- und 15- |
+-----+----------------+------------+-------------+


Stecker 2
+++++++++
(Leitung#: Zahl = Leitungsnummer, +/- = Plus- oder Minus-Pol)

+-----+----------------+------------+-------------+
| Pin | Ader-Farbe     | Ltg.-Dicke | Leitung #   |
+=====+================+============+=============+
| 1   | gelb           | 1,5 mm²    | 4+          |
+-----+----------------+------------+-------------+
| 2   | blau           | 1,5 mm²    | 6+          |
+-----+----------------+------------+-------------+
| 3   | weiß oder pink | 1,5 mm²    | 8+          |
+-----+----------------+------------+-------------+
| 4   | grün           | 1,5 mm²    | 10+         |
+-----+----------------+------------+-------------+
| 5   | braun          | 1,5 mm²    | 4-          |
+-----+----------------+------------+-------------+
| 6   | rot            | 1,5 mm²    | 6-          |
+-----+----------------+------------+-------------+
| 7   | schwarz        | 1,5 mm²    | 8-          |
+-----+----------------+------------+-------------+
| 8   | grau           | 1,5 mm²    | Daten       |
+-----+----------------+------------+-------------+
| 9   | braun/blau     | 2,5 mm²    | 2+          |
+-----+----------------+------------+-------------+
| 10  | braun/rot      | 2,5 mm²    | 12+ und 14+ |
+-----+----------------+------------+-------------+
| 11  | weiß/rot       | 2,5 mm²    | 2-          |
+-----+----------------+------------+-------------+
| 12  | lila           | 1,5 mm²    | 10-         |
+-----+----------------+------------+-------------+
| 13  | weiß/schwarz   | 2,5 mm²    | 12- und 14- |
+-----+----------------+------------+-------------+

Schaltkasten
------------

Der Schaltkasten ist das Gehirn von LeDi. Hier befindet sich im Schaltschrank die gesamte Steuerung und ein großer Teil der Elektronik.

Schaltschrank
#############

Stromführung
++++++++++++
Im Schaltkasten wird der Strom durch ein normales Zuleitungskabel mit 230V eingeführt. Dieses Endet an einem FI-Schalter. Dier FI-Schalter bringt für den Fall der Fälle eine letzte Sicherheit, dass keine Kurzschlüsse oder Stromschläge zu größeren Problemen oder Verletzungen führen. 
Am Ausgang des FI-Schalter ist zunächst eine Schuko-Steckdose angeschlossen, welche für das Netzteil des Raspberry Pis genutzt wird. Ebenfalls am Ausgang des FI-Schalters angeschlossen sind insgesamt fünf Sicherungsautomaten. Jeder dieser Sicherungsautomaten sichert drei Netzteile ab. Für jeden Sicherungsautomaten gilt anschließend, dass der Ausgang an einem Relais angeschlossen ist, welches ein verzögertes Einschalten der Netzteile ermöglicht. Die Relais werden im Abstand von 2 Sekunden nacheinander eingeschaltet. Hierdurch wird die Spannungsspitze beim Einschalten der Netztteile entzerrt und die Sicherung der Zuleitung zum Schaltkasten löst nicht mehr aus. Nach den Relais führt jedes Relais weiter in drei Schmelzsicherungen und anschließend zu je einem Netzteil. Der Materialliste ist zu entnehmen welche Arten von FI-Schalter, Sicherungsautomaten, Relais und Schmelzsicherungen verwendet wurden.
Als Leitungen für die Schaltschrank-Verkabelung wurden 1,5mm² Leitungen verwendet. Die Ausführung zu den Netzteilen ist je ein "Gummikabel" mit 3x1,5mm² wobei die Leitung zur Erdung ungenutzt ist.

Kasten
++++++++++++
Der Kasten ist ein einfacher Schaltschrank mit IP65 Zertifizierung. Sämtliche Einführungen wurden von unten her durchgeführt und mit Quetsch-Einführungen wurde die Einführung dicht gemacht.

RaspberryPi
++++++++++++
Der RaspberryPi ist an der erwähnte Schuko-Steckdose angeschlossen und wird von dort mit Strom versorgt. Auf dem Raspberry Pi selbst muss der GPIO-Pin 18 (Physischer Port 12) mit der Datenleitung der LED-Strips verbunden werden. Der physische Pin 14 mit der Masse.
An dem GPIO Pin 18 ist dann eine Verbindung zu einem vom Typ SN74HCT125N. Dieser Chip kann das Eingangssignal, welches vom Raspberry Pi mit 3,3V gesendet wird, auf 5V hochtransformieren, sodass die LED-Stips damit "zufriedener" sind. Dies ist ins Besondere unter dem Aspekt der Leitungslänger zu empfehlen. Es KANN auch ohne klappen - ins Besondere bei kurzen Leitungslängen. Es KANN jedoch auch nicht ausreichend sein. Sollte es nicht ausreichend sein, muss vor der ersten LED am Display noch ein Pull-Down-Widerstand eingebracht werden. Dieser zieht Störsignale dann runter und die LEDs können meist das Signal wieder verarbeiten.
Das Schema der Verkabelung des SN74HCT125N ist hierbei wie folgt:

    .. image:: images/ledi-builddisplay/LevelShifter.png        
        :alt: Raspberry Pi SN74HCT125N Level Shifter 3.3V to 5V

Netzteile
##########
Die verwendeten Netzteile, welche die LED-Strips und den SN74HCT125N mit 12A bei 5V mit Strom versorgen, sind im Schaltkasten hinter dem Schaltschrank verbaut. Die Netzteile sind IP67 zertifiziert, können also quasi beliebig viel Wasser ab. Ein Netzteil reicht mit den 12A Leistung für 4 LED Strips, wodurch wir auf eine Gesamtmenge von 15 Netzteilen kommen. Im Falle, dass das Display niemals in feuchten Umgebungen oder im Außenbereich eingesetzt wird, kann man auch ein ungeschütztes Netzteil verwenden. Diese gibt es mit deutlich mehr Leistung bei 5V und sind dadurch erheblich günstiger und vor allem auch leichter. Diese Netzteile sind aber offen mit Lüftungslöchern, wodurch selbst bei leichter Feuchtigkeit ein erhebliches Kurzschlussrisiko besteht.
Die von uns verwendeten Netzteile haben neben der IP67 Zertifizierung auch einen Überlastschutz und einen Kurzschlussschutz. So werden in Regelfall größere Schäden verhindert.

Verkabelung
###########
Die Verkabelung vom Schaltschrank zu den Netzteilen erfolgt in "Gummileitungen". Die Verbindung zwischen Netzteil und den Adern der Leitung ist mit einem Quetschverbinder umgesetzt, welcher nach erfolgreichem Verbinden mit einem Heißluftföhn erhitzt wurde. Hierdurch sind die Quetschverbinder geschrumpft und sind dicht am Kabel anliegend. Nach Herstellerinformationen sollen die Quetschverbinder somit Wasserdicht sein. Zusätzlich zu den Quetschverbindern ist noch ein Schrumpfschlauch über die Verbinder gezogen. Dies dient zum Einen dem weiteren Schutz vor Feuchtigkeit und zum anderen der verbesserten Optik.


Rahmen
######
Der Rahmen des Schaltkastens besteht, wie auch das Display, aus 3030-Aluminiumprofilen (30mm x 30mm Kantenlänge). Diese wurden in entsprechender Größe bereits zugeschnitten bestellt und mit Winkel- und Eckverbindern verbunden. 


Verbindungskabel
----------------
Das Verbindungskabel ist ein Kabel für die Verbindung von Anhängern zu Autos. Hierdurch ist es auch wasserfest. Die Kabel haben insgesamt 13 Adern wovon 9 Stück 1,5mm² dick sind und 4 Stück 2,5mm². Wichtig: Es gibt diese Kabel auch mit 13*1,5mm². Hier muss unbedingt auf die richtige Ausführung geachtet werden!
An beiden Enden der Kabel ist jeweils ein Stecker montiert. Hierbei wurde die bereits in der Tabelle oben beschriebene Belegung umgesetzt. Durch die Belegung der beiden Kabel ist es auch möglich die Kabel zu vertauschen oder zu kreuzen, da z.B. auf Pin 1 in beiden Fällen Plus anliegt, auf Pin 5 in beiden Fällen Minus und die Pins 9, 10, 11 und 13 die dickeren Kabel sind sowie Pin 8 die Datenleitung ist. Die Datenleitung selbst ist im Schaltkasten an beiden Steckdosen angelegt, im Panel aber nur an einer. So läuft das Signal sicher immer durch eine Leitung zum Display ohne dass Interferenzen o.ä. entstehen.
Wir haben zwei Ausführungen der Verbindungskabel hergestellt. Eine 1m-Version für den Fall, dass Display und Schaltkasten nahe beieinander stehen und eine 5m-Version für den Fall, dass diese etwas entfernt voneinander stehen (z.B. bei Schwimmbecken).

Materialliste
-------------
Folgt...
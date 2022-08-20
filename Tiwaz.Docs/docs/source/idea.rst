Idee
====

Die Idee zu Tiwaz entstand bei einem Unterwasserhockey Turnier, bei dem eine Anzeigetafel mit Hilfe eines normalen Fernsehers umgesetzt wurde. Dies brachte jedoch drei erhebliche Probleme mit sich:
1. Wenn die Sonne auf den Bildschirm schien, war dieser nicht mehr lesbar.
2. Wenn man aus einem halbwegs spitzen Winkel auf den Bildschirm geguckt hat, war nichts zu erkennen.
3. Es gab Reflextionen von hellen Wänden und anderen Objekten, welche teile des Bildschirms nicht mehr lesbar machten.

Es entstand die Idee, dass es doch möglich sein muss mit relativ wenig Aufwand ein Display auf Basis von LEDs zu erstellen, welches mit den drei genannten Punkten auf Grund der höheren Leuchtstärke keine Probleme haben sollte.
So wurde überlegt und recherchiert. Im Ergebniss kam heraus, dass wir auf Basis von WS2812B LED Strips ein ca. 1 Meter breites Display erschaffen wollen. Die Steuerung kann ein Raspberry Pi übernehmen, sodass hier kostengünstig und gleichzeitig flexibel gearbeitet werden kann.
Das Sammeln von gewünschten Eigenschaften brachte folgende Liste hervor:
- Transportabel => nicht zu groß, nicht zu schwer
- Auch aus der Entfernung noch erkennbar => nicht zu klein
- Spritzwasser und Regengeschützt => Keine Probleme mit Regen oder Spritzwasser bei Wassersportarten
- Keine 230V im Display selbst, da dies ggf. in Wassernähe aufgebaut wird und im schlimmsten Fall ins Wasser fallen kann => Trennung von Display und Schaltung
- Wasserdichte Netzteile
- Bedienung via Browser von jedem beliebigen Gerät
- Verbindung via WLAN möglichst aufgespannt durch den Raspberry Pi selbst

Note: Siehe Materialliste für mehr Details zu den Komponenten.
So wurde dann eine erste Idee und ein erster Prototyp entwickelt, der die Machbarkeit belegen sollte. Hierzu wurde die LED Strips, 60 Stück á 96 LEDs, auf eine Polystyrol-Platte mit Sanitärsilikon geklebt. Polystyrol ist leicht und robust (in entsprechender Dicke). Alle Anschlüsse der Strips wurden anschließend mit Kabeln verlötet und mit den 20 (!) Netzteilen verbunden.
Wir haben uns dazu entschieden Wasserdichte Netzteile zu verwenden, welche allerdings auf Grund fehlender Belüftung in der Leistung reduziert sind. Daher benötigen wir einige Netzteile und nicht nur eines.
Nachdem auch die Netzteile angeschlossen und über extra Sicherungen mit dem Stromnetz verbunden waren, konnten wir den Prototypen testen. Hier stellte sich heraus, dass die Netzteile nacheinander mit Strom versorgt werden müssen, da der Einschaltstrom aller Netzteile zur gleichen Zeit zu hoch war und die Haupt-Sicherung (FI Schutzschalter) ausgelöst hat.
Nachdem dies gelöst war, konnten die LED Strips über einen Raspberry Pi erfolgreich angesteuert werden.
Der Prototyp war somit erfolgreich und es gab einige "Lessons-Learned":
- Polystyrol als Träger ist zu unhandlich. Es ist nicht stabil genug um einen Rahmen drum herum zu tragen und zu unpraktisch um nur verkleidet zu werden. Außerdem ist es zu weich und würde schon bei kleineren Stößen Beschädigungen aufweisen.
- Es sind weniger Netzteile erforderlich
- Relais werden beim Einschalten benötigt um ein Auslösen der Sicherung zu vermeiden

Daraus haben sich folgende Änderungen ergeben:
- Als Träger nutzen wir eine Kunststoff-Platte (z.B. Schreibtischstuhlunterlage)
- Als Schutz und Diffusor vor den LEDs nutzen wir eine Schreibtischstuhlunterlage
- Als Rahmen nutzen wir Aluminium Doppel-Profile (30x60, Nut 8 B-Type)
- Als Verbinder zwischen Display und Schaltkasten nutzen wir zwei Anhängerkupplungsstecker und zwei Kabel mit 9*1,5mm² und 4*2,5mm² à 3m.
- Am Rahmen wird im oberen Bereich eine Kette befestigt zum Aufhängen
- Am Rahmen werden im unteren Bereich Füße installiert. Im Idealfall abnehmbar um die Gesamttiefe des Displays nicht zu erhöhen zwecks Transport.
- Die einzelen Teile des Rahmes werden mit dünnen Stahlseilen oder Splints gegen auseinanderfallen gesichert (zusätzlich zu den regulären Schraubverbindern)
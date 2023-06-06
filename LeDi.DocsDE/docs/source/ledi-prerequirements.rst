Voraussetzungen für LeDi auf einem RaspberryPi installieren
===========================================================

Diese Anleitung führt durch die Installation von LeDi auf einem RaspberryPi vom Start an. Für die Erstellung dieser Anleitung wurde ein RaspberryPi 3 genutzt, aber sie ist auch für einen RapsberryPi 4 anwendbar. Für einen RaspberryPi 2 werden zum Teil andere Lösungen benötigt wie z.B. für die Erstellung des Wifi Hotspots - z.B. durch Nutzung eines extra USB-WLAN-Sticks oder dem Hinzufügen eines WLAN Accesspoints zum Netzwerk des Displays. Wir empfehlen ausschließlich einen RaspberryPi 3 oder 4 mit mindestens 4GB RAM zu nutzen. Wir werden nicht auf nährere Details eingehen, wie eine Installation für andere Modelle möglich ist. Der RaspberryPi 1 wird nicht funktionieren, da dieser kein ARM7 Prozessor besitzt und somit .Net 6 nicht unterstützt.

RaspberryPi installieren
########################

Als allererstes muss der RapsberryPi installiert werden. Dazu müssen wir zunächst den Raspberry Pi Imager herunterladen. 

* Daher gehen wir auf `https://raspberrypi.com/software: <https://raspberrypi.com/software>`_ und klicken auf "Download for Windows". 

    .. image:: images/ledi-prerequirements/01-Download.png
        :width: 800px
        :height: 600px
        :alt: Raspberry Pi Imager download page


* Anschließend führend wir die heruntergeladene Datei ``imager_x.y.z.exe`` aus. 

    .. image:: images/ledi-prerequirements/02-RaspberryPiImager.png
        :alt: Raspberry Pi Imager executable
        :scale: 50 %


* Nun führen wir die Datei aus und folgen dem sehr simplen Setup. Zunächst klicken wir einfach auf **Install**.

    .. image:: images/ledi-prerequirements/03-Setup1.png
        :alt: Raspberry Pi Imager Setup Step 1
        :scale: 50 %


* Und das war es auch schon. Da wir direkt weiter machen wollen, wird der Haken ``Run Raspberry Pi Imager`` aktiviert und ``Finish`` geklickt.

    .. image:: images/ledi-prerequirements/04-SetupDone.png
        :alt: Raspberry Pi Imager executable
        :scale: 50 %


* Nun startet der Raspberry Pi Imager. Hier klicken wir zunächst auf ``Choose OS``.

    .. image:: images/ledi-prerequirements/05-Imager.png
        :alt: Raspberry Pi Imager Choose OS
        :scale: 50 %


* In dem Dialog wählen wir zuerst ``Raspberry Pi OS (other)`` aus.

    .. image:: images/ledi-prerequirements/06-SelectOs1.png
        :alt: Raspberry Pi Imager Select other OS
        :scale: 50 %


* Anschließend ``Raspberry Pi OS Lite (32-bit)``.

    .. image:: images/ledi-prerequirements/07-SelectOs2.png
        :alt: Raspberry Pi Imager Select Raspberry Pi OS Lite (32-Bit)
        :scale: 50 %


* Nun zeigt die Schaltfläche, die zuvor ``Choose OS`` angezeigt hat die ausgewählte Option an. Spätestens jetzt muss die Micro-SD-Karte in den Kartenleser des PCs eingeführt werden. Wenn dies geschehen ist wählen wir ``Choose Storage``.

    .. image:: images/ledi-prerequirements/08-ImagerAfterOsSelect.png
        :alt: Raspberry Pi Imager After Choose OS
        :scale: 50 %


* Hier werden nun alle Wechselmedien angezeigt, die der Imager als SD-Karte identifiziert hat. Es kann gelegentlich vorkommen, dass auch andere Medien wie USB-Sticks hier aufgelistet werden. Diese können zwar im folgenden geflashed werden, aber können dann nicht am Rapsberry Pi betrieben werden. Sollte in der Liste mehr als ein Element sein und nicht klar sein welches die SD-Karte ist, müssen alle USB-Sticks etc. vom PC getrennt werden. Bei der Identifizierung hilft meist zunächst die Größe des Mediums (hier im Beispiel 8GB). Erst in zweiter Reihe hilft ggf. der Name - leider auch nicht immer. Es muss jedenfalls nun die Micro-SD-Karte aus der Liste ausgewählt werden.

    .. image:: images/ledi-prerequirements/09-SelectStorage.png
        :alt: Raspberry Pi Imager select storage
        :scale: 50 %


* Nachdem das Medium ausgewählt wurde, muss im Raspberry Pi Imager das Zahnrad-Symbol angeklickt werden um weitere Einstellungen vornehmen zu können.

    .. image:: images/ledi-prerequirements/10-ImagerAfterStorageSelect.png
        :alt: Raspberry Pi Imager after storage select
        :scale: 50 %


* Hier müssen nun die folgenden Einstellungen angepasst werden:

    1. Der Hostname sollte auf einen passenden Namen gesetzt werden. ``ledi`` bietet sich hier an.
    2. SSH sollte aktiviert werden. Die Passwort Authentifizierung ist ausreichend.
   
        .. image:: images/ledi-prerequirements/11-AdvancedOptions1.png
            :alt: Raspberry Pi Imager advanced options 1
            :scale: 50 %

    3. Der Benutzername und das Kennwort muss gesetzt werden. Als Benutzer bietet sich hier ``ledi`` an. Das Kennwort kann frei vergeben werden.
   
        .. image:: images/ledi-prerequirements/12-AdvancedOptions2.png
            :alt: Raspberry Pi Imager advanced options 2
            :scale: 50 %

    4. Zuletzt noch die passende Zeitzone und das passende Tastaturlayout setzen (z.B. ``Europe/Berlin`` und ``de``).

        .. image:: images/ledi-prerequirements/13-AdvancedOptions3.png
            :alt: Raspberry Pi Imager advanced options 3
            :scale: 50 %

    5. Abschließend den Dialog mit ``Save`` bestätigen.

* Nun befinden wir uns wieder im Raspberry Pi Imager Fenster. Dort starten wir mit einem Klick auf ``Write`` den Flash-Vorgang, in dem das Raspberry Pi OS auf die SD-Karte kopiert wird und die von uns getätigten Einstellungen gesetzt werden.

    .. image:: images/ledi-prerequirements/14-ImagerAfterAdvancedOptions.png
        :alt: Raspberry Pi Imager after advanced options
        :scale: 50 %

* Wir müssen nun noch einmal bestätigen, dass wir die eingelegte Micro-SD-Karte unwiderruflich mit dem Image überschreiben wollen.

    .. image:: images/ledi-prerequirements/15-ConfirmWrite.png
        :alt: Raspberry Pi Imager confirm overwrite
        :scale: 50 %

* Nun warten wir, bis der Schreibvorgang abgeschlossen ist.

    .. image:: images/ledi-prerequirements/16-Writing.png
        :alt: Raspberry Pi Imager writing
        :scale: 50 %

* Abschließend erscheint eine entsprechende Meldung.

    .. image:: images/ledi-prerequirements/17-ImagerImageingDone.png
        :alt: Raspberry Pi Imager success
        :scale: 50 %

* Nun ist die Installation des Raspberry Pis mit dem Raspberry Pi OS Lite Image auf die SD-Karte abgeschlossen.
* Lege nun die Micro-SD-Karte in den Raspberry Pi ein und start diesen. Der erste Start kann einige Minuten dauern. Sofern der Raspberry Pi via Netzwerkkabel mit dem Netzwerk verbunden ist, kann dieser anschließend via SSH erreicht werden oder alternativ einfach über einen direkt angeschlossenen Bildschirm/Tastatur/Maus bedient werden.
* Für Option via SSH: Um via SSH zu verbinden muss kann man via ``ping -4 ledi`` die IP-Adresse herausfinden. Alternativ kann man auch direkt via hostnamen verbinden. Dies geschieht mit dem Programm PuTTY (`Download hier: <https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html>`_ unter ``putty-64bit-0.78-installer.msi``). Nach der Installation zu der IP-Adresse bzw. ``ledi`` verbinden und die im Raspberry Pi Imager angegebenen Benutzerdaten zum Anmelden nutzen.
* Nachdem der erste Start erfolgreich durchgeführt wurden, aktualisieren wir noch das Betriebssystem mit folgenden Befehlen:
    .. parsed-literal::
        sudo apt update
        sudo apt upgrade

* Bei der ersten Ausführung eines ``sudo``-Befehls erfolgt immer eine zusätzliche Passwortabfrage. Dies geschieht auch, wenn man einige Zeit keine ``sudo``-Befehle ausgeführt hat. Ggf. erfolgt auch eine Rückfrage, ob man die Installation fortsetzen möchte, weil weitere X MB Speicherplatz benötigt werden. Diese kann mit einem Y bestätigt werden.
* Der ``sudo apt upgrade`` Befehl wird ggf. eine ganze Weile dauern.
* Nun ist der Raspberry Pi in der Basis installiert und wir können nun die Installation von LeDi starten.

.Net 7 installieren:
####################
Jetzt werden wir .Net 7 installieren. Dazu gehen wir zunächst auf https://dotnet.microsoft.com/en-us/download/dotnet/7.0.
Auf dieser Seite unter ``SDK 7.0.xx`` in der Tabelle den Eintrag ``Linux``. Dort die Binary ``Arm32`` anklicken. Nun die URL, die als ``Direktlink`` ("direct link") bezeichnet ist, kopieren (z.B. https://download.visualstudio.microsoft.com/download/pr/5a24144e-0d7d-4cc9-b9d8-b4d32d6bb084/e882181e475e3c66f48a22fbfc7b19c0/dotnet-sdk-7.0.403-linux-arm.tar.gz). Anschließend zur Raspberry Pi Konsole wechseln und folgendes ausführen:

.. parsed-literal::
    wget <url from above>

Anschließend die folgenden Befehle ausführen. Dabei den Dateinamen in der ersten Zeile mit dem Dateinamen der zuvor heruntergeladenen Datei ersetzen.

.. parsed-literal::
    DOTNET_FILE=dotnet-sdk-7.0.302-linux-arm.tar.gz
    export DOTNET_ROOT=$(pwd)/.dotnet

    mkdir -p "$DOTNET_ROOT" && tar zxf "$DOTNET_FILE" -C "$DOTNET_ROOT"

    export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
    echo "export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools" > .bashrc

Herzlichen Glückwunsch. .Net 7.0 ist nun installiert. Dies kann über den Befehl ``dotnet --version`` geprüft werden. Erfolgt dort eine Ausgabe mit der Versionsnummer, so war die installation erfolgreich.


Einrichten des WLAN AccessPoints
################################
In diesem Absatz wird der RaspberryPi zu einem eigenständigen Hotspot. Dieser Schritt kann übersprungen werden, wenn ein externer WLAN AccessPoint genutzt werden soll oder kein AccessPoint benötigt wird da der RaspberryPi via Ethernet Kabel angeschlossen wird. Diesen Schritt zu überspringen macht LeDi jedoch weniger mobil, da nicht einfach der Stecker in die Steckdose gesteckt werden kann und anschließend losgelegt werden kann.
Um den RaspberryPi als WLAN-AccessPoint einzurichten, müssen die folgenden Schritte durchgeführt werden. Um einen (permanenten) Verbindungsabbruch während der Einrichtung zu vermeiden, sollte der RaspberryPi via Netzwerkkabel mit dem Netzwerk verbunden sein oder direkt via HDMI und Tastatur bedient werden. Zusammengefasst werden wir den RaspberryPi zu einem DNS Server (dnsmasq) und DHCP Server (dhcpd) machen und die Applikation hostapd installieren um die Hotspot/AccessPoint-Funktionalität bereit zu stellen, über den sich anschließend die Clients verbinden können.

Wi-Fi aktivieren
****************
Sofern beim Login folgende Meldung angezeigt wurde, bitte noch den folgenden Abschnitt befolgen. Wenn nicht, diesen Abschnitt überspringen.

.. parsed-literal::
    Wi-Fi is currently blocked by rfkill.
    Use raspi-config to set the country before use.

Also machen wir, was dort steht. Wir führen folgenden Befehl aus:

.. parsed-literal::
    sudo raspi-config

Anschließend öffnet sich das Raspbery Pi Software Configuration Tool. Hier gehen wir auf den ersten Punkt ``System Settings``. Anschließend wählen wir ``S1 Wireless LAN`` aus. Nun ``DE Germany`` für Deutschland. Die anschließende Meldung bestäigen wir noch mit ``OK`` und wenn ``Please enter SSID`` erscheint, gehen wir auf ``Cancel``. Im Hauptmenü dann auf ``Finish``.

Hostapd und Dnsmasq installieren
*********************************
Folgende Befehle ausführen um hostapd und dnsmasq zu installieren und den hostapd service zu konfigurieren. dhcpd ist in der Regel bereits vorinstalliert und braucht nicht installiert zu werden.

.. parsed-literal::
    sudo apt install -y hostapd dnsmasq
    sudo systemctl unmask hostapd
    sudo systemctl enable hostapd

Nachdem die installation ausgeführt wurde, müssen die Dienste einmal angehalten werden um diese zu konfigurieren:

.. parsed-literal::
    sudo systemctl stop hostapd
    sudo systemctl stop dnsmasq

DHCP konfigurieren (dhcpcd)
***************************
Nun installieren wir das private Netzwerk für den WLAN Netzwerkadapter des RaspberryPi. Wir wählen ein Subnetz in einem privaten IP Segment und ein /24 (255.255.255.0) Subnetz. In diesem Fall nutzen wir das Subnetz 10.10.100.0/24 und damit die IP 10.10.100.1 für die IP-Addresse des RaspberryPi. Es kann jedoch auch jedes andere Subnetz und jede andere darin liegende IP genutzt werden.
Nun die IP Konfiguration wie folgt konfigurieren:

.. parsed-literal::
    sudo nano /etc/dhcpcd.conf

Folgende Zeilen am Ende der Datei hinzufügen:

.. parsed-literal::
    interface wlan0
    nohook wpa_supplicant
    static ip_address=10.10.100.1/24
    static domain_name_servers=10.10.100.1

Die Datei mit Strg+O speichern und mit Strg+X die Datei schließen.

DHCP konfigurieren (dnsmasq)
****************************
Jetzt konfigureren wir den DHCP Server in dnsmasq um den verbindenden Clients eine IP bereit zu stellen. Zunächst eine neue config erstellen:

.. parsed-literal::
    sudo mv /etc/dnsmasq.conf /etc/dnsmasq.conf.orig
    sudo nano /etc/dnsmasq.conf

Nun folgendes einfügen:

.. parsed-literal::
    interface=wlan0
    dhcp-range=10.10.100.11,10.10.100.254,255.255.255.0,1h
    domain=intern
    listen-address=10.10.100.1
    listen-address=127.0.0.1
    local=/intern/

Die Datei mit Strg+O speichern und mit Strg+X schließen.
Diese Zeilen bedeuten, dass wir die IPs 10.10.100.11 bis 10.10.100.254 in dem Subnetz 255.255.255.0 mit einer Lease Time von einer Stunde an die verbindenden Clients vergeben. Die ersten 10 IPs lassen wir für Infrastrukturdienste frei. Eventuell brauchen wir die ja mal...
Die Konfiguration definiert auch, dass andere Systeme als das lokale System den dnsmasq-service auf dem RaspberryPi nutzen um DNS Addressen aufzulösen. Auch die "intern"-Domain soll von niemand anderen als dem lokalen dnsmasq-Server aufgelöst werden für den Fall dass der RaspberryPi einen Uplink z.B. zum Internet bekommt.

DNS konfigurieren (dnsmasq)
***************************
Jetzt konfigurieren wir die DNS Auflösung. Dazu die hosts-Datei editieren:

.. parsed-literal::
    sudo nano /etc/hosts

Folgende Zeilen am Ende hinzufügen:

.. parsed-literal::
    10.10.100.1	ledi.intern
    10.10.100.1	board board.intern

Die letzte Zeile bedeutet, dass das System den Alias "board" erhält und so von den Clients via Browser aufgerufen werden kann. Es können auch andere Namen verwendet werden. Seit kreativ, so wie ihr es braucht oder haben möchtet.
Die Datei mit Strg+O speichern und mit Strg+X schließen.

WLAN AccessPoint konfigurieren (hostap)
***************************************
Im folgenden wird der WLAN AccessPoint selbst konfiguriert:

.. parsed-literal::
    sudo nano /etc/hostapd/hostapd.conf

Folgende Zeilen in die Datei einfügen:

.. parsed-literal::
    interface=wlan0
    hw_mode=g
    channel=7
    wmm_enabled=0
    macaddr_acl=0
    auth_algs=1
    ignore_broadcast_ssid=0
    wpa=0
    ssid=LeDi

Die Datei mit Strg+O speichern und mit Strg+X schließen.
Dies konfiguriert den Netzwerkadapter wlan0. Der Name (SSID) des WLANs heißt "LeDi" und die Verbindung wird unverschlüsselt sein. So kann sich jeder zum RaspberryPi verbinden.

Die neue Konfiguration muss nun noch beim init geladen werden:

.. parsed-literal::
    sudo nano /etc/default/hostapd

Ersetze die Zeile **#DAEMON_CONF=""** mit **DAEMON_CONF="/etc/hostapd/hostapd.conf"** um das führende # zu entfernen und den Pfad zur neuen Konfigurationsdatei hinzuzufügen.
Die Datei mit Strg+O speichern und mit Strg+X schließen.

Abschluss
*********
Die Dienste (neu)starten:

.. parsed-literal::
    sudo systemctl restart dhcpcd
    sudo systemctl start hostapd
    sudo systemctl start dnsmasq

Wenn ihr nun mit einem Tablet oder Smartphone auf WLAN-Suche geht, findet ihr das WLAN "LeDi", welches vom Raspberry Pi bereitgestellt wird.
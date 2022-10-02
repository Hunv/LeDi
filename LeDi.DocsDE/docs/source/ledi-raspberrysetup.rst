LeDi auf einem RaspberryPi installieren
=======================================

Diese Anleitung führt durch die Installation von LeDi auf einem RaspberryPi vom Start an. Für die Erstellung dieser Anleitung wurde ein RaspberryPi 3 genutzt, aber sie sollte auch für einen RapsberryPi 4 anwendbar sein. Für einen RaspberryPi 1 und 2 werden zum Teil andere Lösungen benötigt wie z.B. für die Erstellung des Wifi Hotspots - z.B. durch Nutzung eines extra USB-WLAN-Sticks oder dem Hinzufügen eines WLAN Accesspoints zum Netzwerk des Displays. Wir empfehlen ausschließlich einen RaspberryPi 3 oder 4 mit mindestens 4GB RAM zu nutzen. Wir werden nicht auf nährere Details eingehen, wie eine Installation für andere Modelle möglich ist.

RaspberryPi installieren
########################
Dieser Absatz ist in Entstehung!
This section needs to be improved to go into more details (i.e. hostname, IP, configure User, Wifi and SSH on image creation).
To install the RapsberryPi, follow the regular process of installing it. Use a 32 Bit "Lite" image. The lite image does not have any desktop environment installed, which saves resources because it is not needed.
Also run the following commands after installing OS:
.. code-block:: bash
    :linenos:

    sudo apt update
    sudo apt upgrade



.Net 6 installieren:
####################
Jetzt werden wir .Net 6 installieren. Dazu gehen wir zunächst auf https://dotnet.microsoft.com/en-us/download/dotnet/6.0.
Auf dieser Seite unter "Linux" OS die Binary "Arm32" auswählen. Nun die URL, die als "Direktlink" ("direct link") bezeichnet ist, kopieren (z.B. https://download.visualstudio.microsoft.com/download/pr/5a24144e-0d7d-4cc9-b9d8-b4d32d6bb084/e882181e475e3c66f48a22fbfc7b19c0/dotnet-sdk-6.0.400-linux-arm.tar.gz). Anschließend zum Raspberry verbinden und folgendes ausführen:
.. code-block:: bash
    :linenos:
    wget <url from above>

Anschließend die folgenden Befehle ausführen. Dabei den Dateinamen in der ersten Zeile mit dem Dateinamen der zuvor heruntergeladenen Datei ersetzen.
.. code-block:: bash
    :linenos:
    DOTNET_FILE=dotnet-sdk-6.0.100-linux-x64.tar.gz
    export DOTNET_ROOT=$(pwd)/.dotnet

    mkdir -p "$DOTNET_ROOT" && tar zxf "$DOTNET_FILE" -C "$DOTNET_ROOT"

    export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

Herzlichen Glückwunsch. .Net 6.0 ist nun installiert.


Einrichten des WLAN AccessPoints
################################
In diesem Absatz wird der RaspberryPi zu einem eigenständigen Hotspot. Dieser Schritt kann übersprungen werden, wenn ein externer WLAN AccessPoint genutzt werden soll oder kein AccessPoint benötigt wird da der RaspberryPi via Ethernet Kabel angeschlossen wird. Diesen Schritt zu überspringen macht LeDi jedoch weniger mobil, da nicht einfach der Stecker in die Steckdose gesteckt werden kann und anschließend losgelegt werden kann.
Um den RapsberryPi als WLAN-AccessPoint einzurichten, müssen die folgenden Schritte durchgeführt werden. Um einen (permanenten) Verbindungsabbruch während der Einrichtung zu vermeiden, sollte der RaspberryPi via Netzwerkkabel mit dem Netzwerk verbunden sein. Zusammengefasst werden den RaspberryPi zu einem DNS Server (dnsmasq) und DHCP Server (dhcpd) machen und hostapd installieren um die Hotspot/AccessPoint-Funktionalität bereit zu stellen, über den sich anschließend die Clients verbinden können.

Hostapd und Dnsmasq installieren:
*********************************
Folgende Befehle ausführen um hostapd und dnsmasq zu installieren und den hostapd service zu konfigurieren. dhcpd ist in der Regel bereits vorinstalliert und braucht nicht installiert zu werden.
.. code-block:: bash
    :linenos:
    sudo apt install hostapd -y
    sudo apt install dnsmasq -y
    sudo systemctl unmask hostapd
    sudo systemctl enable hostapd

Nachdem die installation ausgeführt wurde, müssen die Dienste einmal angehalten werden um diese zu konfigurieren:
.. code-block:: bash
    :linenos:
    sudo systemctl stop hostapd
    sudo systemctl stop dnsmasq

DHCP konfigurieren (dhcpcd)
***************************
Nun installieren wir das private netwerk für den WLAN Netzwerkadapter des RaspberryPi. Wir wählen ein Subnetz in einem privaten IP Segment und ein /24 (255.255.255.0) Subnetz. In diesem Fall nutzen wir das Subnetz 10.10.100.0/24 und damit die IP 10.10.100.1 für die IP-Addresse des RaspberryPi. Es kann jedoch auch jedes andere Subnetz und jede andere darin liegende IP genutzt werden.
Nun die IP Konfiguration wie folgt konfigurieren:
.. code-block:: bash
    :linenos:
    sudo nano /etc/dhcpcd.conf

Folgende Zeilen am Ende der Datei hinzufügen_
.. code-block:: text
    :linenos:
    interface wlan0
    nohook wpa_supplicant
    static ip_address=10.10.100.1/24
    static domain_name_servers=10.10.100.1

Die Datei mit Strg+O speichern und mit Strg+X schließen.

DHCP konfigurieren (dnsmasq)
****************************
Jetzt konfigureren wir den DHCP Server in dnsmasq um den verbindenden Clients eine IP bereit zu stellen. Zunächst eine neue config erstellen:
.. code-block:: bash
    :linenos:
    sudo mv /etc/dnsmasq.conf /etc/dnsmasq.conf.orig
    sudo nano /etc/dnsmasq.conf

Nun folgendes einfügen:
.. code-block:: text
    :linenos:
    interface=wlan0
    dhcp-range=10.10.100.11,10.10.100.254,255.255.255.0,1h
    domain=intern
    listen-address=10.10.100.1
    listen-address=127.0.0.1
    local=/intern/

Die Datei mit Strg+O speichern und mit Strg+X schließen.
Diese Zeilen bedeuten, dass wir die IPs 10.10.100.11 bis 10.10.100.254 in dem Subnetz 255.255.255.0 mit einer lease time von einer Stunde an die verbindenden Clients vergeben. Die ersten 10 IPs lassen wir für Infrastructurdienste frei. Eventuell brauchen wir die ja mal...
Die Konfiguration definiert auch, dass andere Systeme als lokale System den dnsmasq-service auf dem RaspberryPi nutzen um DNS Addressen aufzulösen. Auch die "intern"-Domain soll von niemand anderen als dem lokalen dnsmasq-Server aufgelöst werden für den Fall dass der RaspberryPi einen Uplink z.B. zum Internet bekommt.

DNS konfigurieren (dnsmasq)
***************************
Jetzt konfigurieren wir die DNS Auflösung. Dazu die hosts-Datei editieren:
.. code-block:: bash
    :linenos:
    sudo nano /etc/hosts

Folgende Zeilen am Ende hinzufügen:
.. code-block:: text
    :linenos:
    10.10.100.1	ledi.intern
    10.10.100.1	board board.intern

Die letzte Zeile bedeutet, dass das system den Alias "board" erhält und so von den Clients via Browser aufgerufen werden kann. Es können auch andere Namen verwendet werden. Seit kreativ, so wie ihr es braucht.
Die Datei mit Strg+O speichern und mit Strg+X schließen.

WLAN AccessPoint konfigurieren (hostap)
***************************************
Im folgenden wird der WLAN AccessPoint selbst konfiguriert:
.. code-block:: bash
    :linenos:
    sudo nano /etc/hostapd/hostapd.conf

Folgende Zeilen in die Datei einfügen:
.. code-block:: text
    :linenos:
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

Die neue config muss nun noch beim init geladen werden:
.. code-block:: bash
    :linenos:
    sudo nano /etc/default/hostapd

Ersetze die Zeile **#DAEMON_CONF=""** mit **DAEMON_CONF="/etc/hostapd/hostapd.conf"** um das führende # zu entfernen und den Pfad zur neuen config file hinzuzufügen.
Die Datei mit Strg+O speichern und mit Strg+X schließen.

Abschluss
*********
Die Dienste (neu)starten:
.. code-block:: bash
    :linenos:
    sudo systemctl restart dhcpcd
    sudo systemctl start hostapd
    sudo systemctl start dnsmasq
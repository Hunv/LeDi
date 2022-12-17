LeDi Server installieren
========================

Installation
------------

Die Installation von LeDi besteht im Wesentlichen aus zwei Teilen:

1. .Net 6.0 als Framework
2. ws2811.so Datei kompilieren
3. LeDi selbst

.Net 6.0 installieren
---------------------
.Net wurde bereits in dem Abschnitt "Voraussetzungen für LeDi auf einem RaspberryPi installieren" installiert, sodass wir uns hier nun nur noch um LeDi kümmern müssen.

ws2811.so Datei kompilieren
---------------------------
die ws2811.so Datei ist eine Datei, die nachher die LEDs ansteuert. Dazu zunächst scons, git und gcc installieren:

.. code-block:: sh
    :linenos:
    sudo apt -y install scons gcc git

Anschließend das Github Repository herunterladen und die Datei aus den sourcen erstellen

.. code-block:: sh
    :linenos:
    cd ~
    git clone https://github.com/jgarff/rpi_ws281x.git
    cd ~/rpi_ws281x
    scons
    gcc -shared -o ws2811.so *.o
    sudo mkdir /opt/LeDi.Display
    sudo cp ws2811.so /opt/LeDi.Display/


LeDi herunterladen
---------------------
LeDi kann auf der Github Projekt-Seite heruntergeladen werden. Dazu unter https://github.com/Hunv/LeDi/releases beim aktuellsten Release die "LeDi.zip" herunterladen. Sofern der RaspberryPi Internetzugriff hat, kann diese Datei auch direkt via folgendem Befehl heruntergeladen werden (die # durch die Versionsnummer ersetzen):

.. code-block:: sh
    :linenos:
    wget https://github.com/Hunv/LeDi/releases/download/v#.#/LeDi.zip

LeDi installieren
---------------------
Nun die Datei LeDi.zip nach /opt/ entpacken und die Berechtigungen auf Ordner und Ports setzen. Außerdem noch ein paar Konfigurationen für .Net:

.. code-block:: sh
    :linenos:
    sudo unzip-Befehl....
    sudo chmod -R 744 /opt/LeDi*
    sudo adduser ledi kmem
    sudo setcap CAP_NET_BIND_SERVICE=+eip /home/ledi/.dotnet/dotnet
    dotnet dev-certs https --trust

Jetzt erstellen wir ein Serverzertifikat für den Webserver (die roten Meldungen beim zweiten Befehl ignorieren):

.. code-block:: sh
    :linenos:
    sudo /home/ledi/.dotnet/dotnet dev-certs https --clean
    sudo /home/ledi/.dotnet/dotnet dev-certs https --verbose

Anschließend müssen die einzelnen Komponenten nur noch als Service registriert werden. Dazu zunächst folgenden Befehl ausführen um eine neue Datei zu erstellen: ``sudo nano /etc/systemd/system/ledi.server.service``. Dort dann folgenden Text einfügen:

.. code-block:: text
    :linenos:
    [Unit]
    Description=ledi.server
    After=network.target
    
    [Service]
    ExecStart=/home/ledi/.dotnet/dotnet /opt/LeDi.Server/LeDi.Server.dll
    WorkingDirectory=/opt/LeDi.Server/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Jetzt mit Strg+W die Datei speichern und mit Strg+X die Datei schließen. Dies wiederholen wir jetzt für das Display und den WebClient:
``sudo nano /etc/systemd/system/ledi.display.service``. Dort dann folgenden Text einfügen:

.. code-block:: text
    :linenos:
    [Unit]
    Description=LeDi.Display
    After=network.target
    
    [Service]
    ExecStart=/home/ledi/.dotnet/dotnet /opt/LeDi.Display/LeDi.Display.dll
    WorkingDirectory=/opt/LeDi.Display/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Jetzt mit Strg+W die Datei speichern und mit Strg+X die Datei schließen und zuletzt 
``sudo nano /etc/systemd/system/ledi.webclient.service``. Dort dann folgenden Text einfügen:

.. code-block:: text
    :linenos:
    [Unit]
    Description=ledi.webclient
    After=network.target
    
    [Service]
    ExecStart=/home/ledi/.dotnet/dotnet /opt/LeDi.WebClient/LeDi.WebClient.dll
    WorkingDirectory=/opt/LeDi.WebClient/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Mit Strg+W die Datei speichern und mit Strg+X die Datei schließen. 
Nun sind die Konfigurationsdateien für die drei Services vorhanden. Jetzt müssen wir die Services nur noch registrieren und starten:

.. code-block:: sh
    :linenos:
    sudo systemctl daemon-reload
    sudo systemctl enable ledi.server
    sudo systemctl enable ledi.display
    sudo systemctl enable ledi.webclient    
    sudo systemctl start ledi.server
    sudo systemctl start ledi.display
    sudo systemctl start ledi.webclient

Fertig ist die Installation von LeDi. Über einen Browser, der mit LeDi z.B. via WLAN Hotspot verbunden ist, kann LeDi nun unter http://ledi.intern aufgerufen werden.
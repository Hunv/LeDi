LeDi.Server
============

Installation
------------

Die Installation von LeDi besteht im Wesentlichen aus zwei Teilen:

1. .Net 6.0 als Framework
2. LeDi selbst

.Net wurde bereits in dem Abschnitt "RaspberryPi installieren" installiert, sodass wir uns hier nun nur noch um LeDi kümmern müssen.


LeDi herunterladen
---------------------
LeDi kann auf der Github-Seite heruntergeladen werden. Dazu unter https://github.com/Hunv/LeDi/releases beim aktuellsten Release die "LeDi.zip" herunterladen. Sofern der RaspberryPi Internetzugriff hat, kann diese Datei auch direkt via folgendem Befehl heruntergeladen werden (die # durch die Versionsnummer ersetzen):

.. code-block:: console

   wget https://github.com/Hunv/LeDi/releases/download/v#.#/LeDi.zip

LeDi installieren
---------------------
Nun die Datei LeDi.zip nach /opt/LeDi/ entpacken:

.. code-block:: console

    Befehl....

Anschließend müssen die einzelnen Komponenten nur noch als Service registriert werden. Dazu zunächst folgenden Befehl ausführen um eine neue Datei zu erstellen: ``"sudo nano /etc/systemd/system/ledi.server.service"``. Dort dann folgenden Text einfügen:

.. code-block:: console

    [Unit]
    Description=ledi.server
    After=network.target
    
    [Service]
    ExecStart=/opt/ledi/ledi.server/LeDi.Server.dll
    WorkingDirectory=/opt/ledi/ledi.server/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Jetzt mit Strg+W die Datei speichern und mit Strg+X die Datei schließen. Dies wiederholen wir jetzt für das Display und den WebClient:
``"sudo nano /etc/systemd/system/ledi.display.service"``. Dort dann folgenden Text einfügen:

.. code-block:: console

    [Unit]
    Description=ledi.display
    After=network.target
    
    [Service]
    ExecStart=/opt/ledi/ledi.display/LeDi.Display.dll
    WorkingDirectory=/opt/ledi/ledi.display/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Jetzt mit Strg+W die Datei speichern und mit Strg+X die Datei schließen und zuletzt ``"sudo nano /etc/systemd/system/ledi.webclient.service"``. Dort dann folgenden Text einfügen:

.. code-block:: console

    [Unit]
    Description=ledi.webclient
    After=network.target
    
    [Service]
    ExecStart=/opt/ledi/ledi.webclient/LeDi.WebClient.dll
    WorkingDirectory=/opt/ledi/ledi.webclient/
    StandardOutput=inherit
    StandardError=inherit
    Restart=always
    User=ledi
    
    [Install]
    WantedBy=multi-user.target

Mit Strg+W die Datei speichern und mit Strg+X die Datei schließen. 
Nun sind die Konfigurationsdateien für die drei Services vorhanden. Jetzt müssen wir die Services nur noch registrieren und starten:

.. code-block:: console

    sudo systemctl enable ledi.server
    sudo systemctl enable ledi.display
    sudo systemctl enable ledi.webclient
    sudo systemctl start ledi.server
    sudo systemctl start ledi.display
    sudo systemctl start ledi.webclient

Fertig ist die Installation von LeDi.

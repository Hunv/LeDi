LeDi Resourcen
========================

Inhalt
------------
Auf dieser Seite gibt es Links zu externen Resourcen, die bei der Entwicklung und dem Bau geholfen haben sowie hilfreiche Code Snippets zum Testen.


Ansteuerung LEDs
---------------------
Tutorial zum Ansteuern der WS2812B LEDs, allerdings veraltet. Kommentare beachten bzw. den Code unter "Testen der LEDs" auf dieser Seite
https://tutorials-raspberrypi.de/raspberry-pi-ws2812-ws2811b-rgb-led-streifen-steuern/?unapproved=52562&moderation-hash=6d23ab95ca860953672d237f1effe1cb#comment-52562

Bibliothek für den RaspberryPi zur Ansteuerung von WS2812B LEDs (und anderen)
https://github.com/jgarff/rpi_ws281x

Python Umgebung zur Ansteuerung der LEDs
https://github.com/rpi-ws281x/rpi-ws281x-python

C# Beispiel zur Ansteuerung der LEDs
https://github.com/rpi-ws281x/rpi-ws281x-csharp

Ein Projekt zur Ansteuerung anderer Pis von einem zentralen Server zum Steuern von LED strips
https://github.com/tom-2015/rpi-ws2812-server

Testen der LEDs
----------------
Dies hilft zu testen, ob die LEDs korrekt angeschlossen sind.

.. parsed-literal::
    sudo apt-get install gcc make build-essential python-dev git scons swig python-dev pip
    sudo echo "blacklist snd_bcm2835" > /etc/modprobe.d/snd-blacklist.conf
    sudo nano /boot/config.txt
    # Zeile dtparam=audio=on mit einer # auskommentieren
    sudo reboot
    
    sudo apt install python3-dev pip
    mkdir ~/lib
    cd ~/lib
    git clone https://github.com/jgarff/rpi_ws281x
    mkdir ~/py
    cd ~/py
    git clone https://github.com/rpi-ws281x/rpi-ws281x-python
    cd ~/py/rpi-ws281x-python/library
    mkdir lib
    cd ~/py/rpi-ws281x-python/library/lib
    cp -r ~/lib/rpi_ws281x/* ~/py/rpi-ws281x-python/library/lib
    rm -r ~/lib
    sudo scons
    sudo python3 setup.py build
    sudo python3 setup.py install
    sudo pip3 install adafruit-circuitpython-neopixel
    cd ~/py/rpi-ws281x-python
    sudo python3 examples/strandtest.py
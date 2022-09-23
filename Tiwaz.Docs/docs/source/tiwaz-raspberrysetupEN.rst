Install Tiwaz on a RaspberryPi
==============================

This guide will lead you threw the process of installing Tiwaz on a RaspberryPi from scratch. This guide was created using a RaspberryPi 3 but this should also be valid for a RaspberryPi 4. For RaspberryPi 1 and 2 you might need an alternative solution for hosting the Wifi Hotspot by adding a USB wifi stick or by adding a access point to the network. As we only recommend to use a RaspberryPi 3 or 4, we will not go into details how to implement this for RaspberryPi 1 or 2.

Install the RapsberryPi
#######################
This section needs to be improved to go into more details (i.e. hostname, IP, configure User, Wifi and SSH on image creation).
To install the RapsberryPi, follow the regular process of installing it. Use a 32 Bit "Lite" image. The lite image does not have any desktop environment installed, which saves resources because it is not needed.
Also run the following commands after installing OS:
.. code-block:: bash
    :linenos:

    sudo apt update
    sudo apt upgrade



Install .Net 6:
###############
We will install .Net 6 now. To start, go to https://dotnet.microsoft.com/en-us/download/dotnet/6.0.
On that page select at "Linux" OS the Binary "Arm32". Copy the URL, that is the "direct link" (i.e. https://download.visualstudio.microsoft.com/download/pr/5a24144e-0d7d-4cc9-b9d8-b4d32d6bb084/e882181e475e3c66f48a22fbfc7b19c0/dotnet-sdk-6.0.400-linux-arm.tar.gz). Now connect to your RaspberryPi and run 
.. code-block:: bash
    :linenos:
    wget <url from above>

Now run the following commands. Replace the filename at the first line with the filename you just downloaded before.
.. code-block:: bash
    :linenos:
    DOTNET_FILE=dotnet-sdk-6.0.100-linux-x64.tar.gz
    export DOTNET_ROOT=$(pwd)/.dotnet

    mkdir -p "$DOTNET_ROOT" && tar zxf "$DOTNET_FILE" -C "$DOTNET_ROOT"

    export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

Congratiulations. Dotnet is installed.



Setup the Access Point
######################
This section will setup the RaspberryPi as a stand alone Hotspot. You can skip this, if you don't plan to use Tiwaz stand alone. Otherwise if you plan to just take your display and go to the next court, pool, field or whatever - you should implement this.
To connect to the Raspberry via wifi where the RapsberryPi acts as the access point, we need to perform the following steps. To avoid connection loss, connect the RapsberryPi via Ethernet cable to your network or PC. Summarized we will setup the RapsberryPi as a DNS server (dnsmasq) and DHCP server (dhcpcd) and install hostapd to use the wifi adapter to provide a wifi to clients to connect to it.

Install Hostapd and Dnsmasq:
****************************
Run the following commands to install hostapd and dnsmasq and configure hostapd. dhcpcd is already installed by default.
.. code-block:: bash
    :linenos:
    sudo apt install hostapd -y
    sudo apt install dnsmasq -y
    sudo systemctl unmask hostapd
    sudo systemctl enable hostapd

After the installation is done, stop the services, so we can edit the config files after:
.. code-block:: bash
    :linenos:
    sudo systemctl stop hostapd
    sudo systemctl stop dnsmasq

Configure DHCP (dhcpcd)
***********************
Now we setup the private network for the Wifi interface of the Raspberry Pi. We will choose a subnet in one of the private IP segments and a /24 (255.255.255.0) subnetmask. We will take 10.10.100.1 for the IP address of the Rapsberry Pi Wifi interface. Feel free to choose something else.
Edit the IP configuration as follows:
.. code-block:: bash
    :linenos:
    sudo nano /etc/dhcpcd.conf

Add the following lines at the end of the file:
.. code-block:: text
    :linenos:
    interface wlan0
    nohook wpa_supplicant
    static ip_address=10.10.100.1/24
    static domain_name_servers=10.10.100.1

Save the file with Ctrl+O and exit with Ctrl+X

Configure DHCP (dnsmasq)
***********************
Now we configure the DHCP server which will serve the IPs to the connecting clients. Create a new config first.
.. code-block:: bash
    :linenos:
    sudo mv /etc/dnsmasq.conf /etc/dnsmasq.conf.orig
    sudo nano /etc/dnsmasq.conf

Now paste the following content:
.. code-block:: text
    :linenos:
    interface=wlan0
    dhcp-range=10.10.100.11,10.10.100.254,255.255.255.0,1h
    domain=intern
    listen-address=10.10.100.1
    listen-address=127.0.0.1
    local=/intern/

Save the file with Ctrl+O and exit with Ctrl+X.
That lines mean that we will serve IPs from 10.10.100.11 to 10.10.100.254 in the subnet 255.255.255.0 with 1 hour of lease time to connecting clients. We leave the first 10 IPs free for infrastructure devices we may connect somewhen.
It also defines, that also other systems than the local system can connect to the dnsmasq-service to get DNS information as well as that the "intern"-Domain should not be resolved by anyone else than the local system (in case it will have an uplink while it runs).

Configure DNS (dnsmasq)
***********************
Now we configure the DNS resolution. Edit the hosts file:
.. code-block:: bash
    :linenos:
    sudo nano /etc/hosts

Add the lines at the end:
.. code-block:: text
    :linenos:
    10.10.100.1	tiwaz.intern
    10.10.100.1	board board.intern

The last line would mean, that the system will have an alias called "board" that clients are able to use to connect to the page. Feel free to change it or add more/other alias'.
Save the file with Ctrl+O and exit with Ctrl+X.

Configure Access Point (hostap)
*******************************
After that we configure the AccessPoint itself.
.. code-block:: bash
    :linenos:
    sudo nano /etc/hostapd/hostapd.conf

Paste the following lines:
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
    ssid=Tiwaz

Save the file with Ctrl+O and exit with Ctrl+X.
This will configure the interface wlan0. The connection is named "Tiwaz" by default and unencrypted so everyone will be able to connect.

Link the new config from the initscript file:
.. code-block:: bash
    :linenos:
    sudo nano /etc/default/hostapd

Replace the Line **#DAEMON_CONF=""** with **DAEMON_CONF="/etc/hostapd/hostapd.conf"** to remove the leading # and add the path the new config file.
Save the file with Ctrl+O and exit with Ctrl+X.

Finalize
********
(Re-)Start the Services:
.. code-block:: bash
    :linenos:
    sudo systemctl restart dhcpcd
    sudo systemctl start hostapd
    sudo systemctl start dnsmasq
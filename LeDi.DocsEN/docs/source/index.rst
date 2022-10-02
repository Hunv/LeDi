Welcome to LeDi's documentation!
=====================================

**LeDi** is an open source Soft- and Hardware project to primary manage sport matches. It is supported by a LED display to show the current time left, player information, scores etc..

Project at GitHub: `LeDi <https://github.com/Hunv/LeDi>`_

The project has multiple subprojects:

LeDi.Docs
##########
This projects are the documentation like this. It documents how to setup the software and how to build the hardware. It documents the road of custructing the hardware until the final result came out.

LeDi.Server
############
This is the main software component. It is the core of the software architecture and provides an API to provide the current match status. The API can be accessed via a REST API.
LeDi.Server is the only software component that holds the valid match information.
It is created using C# based on .Net 6.0 (LTS).

LeDi.WebClient
###############
This is the software component that provides the frontend to the referees and all other people. The referees can control the match using this component while other people can watch the current standings.
Admins configure the software after installation using the LeDi.WebClient.
It is created in C#, HTML, CSS and Java Script based on Microsoft Balzor (Server) and .Net 6.0 (LTS).

LeDi.Display
#############
This software component controls the LED display, based on WS2812B LEDs. It connects to the LeDi.Server API and gets the information about the behaviour and what needs to be shown from this server also via REST API.
It is created using C# based on .Net 6.0 (LTS).

LeDi.Hardware
##############
This project documents the creation of the display hardware. This is the hardware counterpart to the LeDi.Display project.
It contains the mounting of the case and power supplies as well as the drafts for the electronics and cableing. Especially electronics and calbing should be supported by a trained electican to exclude damage to your health due to electic shocks or other dangers during build and run.


.. note::

   This project and all subprojects are currently under developlement.
   For a roadmap see the roadmap in GitHub.

Index
--------

.. toctree::

   idea
   ledi-server
   Ledi-webclient
   ledi-display
   ledi-hardware
   ledi-docs
   ledi-raspberrysetup

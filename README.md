AdHocSimulator
==============

This utility aims to provide interfaces to simulate an Ad-Hoc WiFi network, so that distributed algorithms based on such Ad-Hoc networks can be easily prototyped and tested.

Currently it provides features:

* Network speed limit, i.e., receiving capability by any device has a global maximum.
* Access limit, i.e., we allow some devices can only see its neighborhood, rather than the overall network.
* Dynamic adding/removal of devices. Some devices can join or leave the network at any time.

The project is implemented in C#.

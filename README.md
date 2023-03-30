# HTC-VIVE-UNITY-TrackerManager
Tool Manager for HTC Vive Trackers

MOTIVATION
The VIVE documentation and the STEAM documentation does not have satifying information about how to manage the Trackers devices, with a lot of templates where not works with HTC-VIVE-PRO devices, so I made this code to manage any device more easely.

USE
1 - Import the file to unity;
2 - Add to an game object;
3 - Set in inspector the number total of total devices connected. Ex.: HMD, Controllers, and obvious Trackers.

In my case I use this manager to point the Trackers in Steam Panel Controller Tracker Setup finded in SteamVR Menu > Configuration > Controls > Manager Trackers.
I use 5 trackers where I setup any of them to an part like, RightShoulder, LeftShoulder, Waist and so on... I made this with only parts I need today, but I will change soon, including all parts.

So, to configure part objects just put the TrackedObject components in object you want to track, after put this gameobject in Tracker Manager list with limb name set.

Cheers

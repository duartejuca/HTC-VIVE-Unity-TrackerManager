HTC-VIVE-UNITY-TrackerManager by Joao Duarte
A tool for managing HTC Vive Trackers in Unity, with an added logging tool for debugging.

Motivation
The official documentation for VIVE and STEAM does not provide satisfactory information on how to manage Tracker devices, and many templates do not work with HTC-VIVE-PRO devices. To address this issue, I developed this tool to make managing any device easier.

How to Use
1. Import the file to Unity.
2. Add it to a game object.
3. In the inspector, set the total number of devices connected, such as HMD, controllers, and trackers.

In my case, I use this tool to configure the trackers in the SteamVR menu under "Configuration > Controls > Manager Trackers" for a full-body setup with five trackers. Currently, I only use the parts that I need, but I plan to include all parts soon.
To configure the tracked objects, simply add the TrackedObject component to the object you want to track and then add this game object to the Tracker Manager list with the limb name set.
In addition, I have included a logging tool for debugging the trackers. This tool creates an XML file with detailed log information.

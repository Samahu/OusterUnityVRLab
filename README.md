# Ouster Unity VR Lab

This project is still in its basic infancy it provides users with the capabilty to experience what Ouster Lidars see in online and offline scenairo:
<img width="2021" height="1020" alt="image" src="https://github.com/user-attachments/assets/f4627e52-1adc-4d1b-bd07-bd8f893bf8db" />

To stream data from a live sensor in Unity you will need the C & C# bindings which were submitted on this PR: https://github.com/ouster-lidar/ouster-sdk/pull/680

Follow the instruction to build the c-native library, build the Unity project, set your sensor ip address on the LiveSensor.cs object.
Finally, launch the `AvatarExperience.unity` scene, Enjory!

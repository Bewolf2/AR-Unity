AR-Unity
========
This is an augmented reality application in the Unity game engine.

Requirements
============
* [Unity Pro 3.5.5](http://www.unity3d.com/) - the game engine. The Pro version is required, since
it uses plugins.
* [ALVAR 2.0.0 SDK](http://virtual.vtt.fi/virtual/proj2/multimedia/alvar/index.html) - the augmented
reality software library.
* [OpenCV 2.4.0](http://opencv.org/) - the third party library dependency of ALVAR.

This project is in two parts:
* **ALVARBridge** - this is the bridge between ALVAR's C++ functions and Unity C# scripts
* **AR-Unity** - this is the Unity application using the augmented reality

ALVARBridge
-----------
This bridge creates an unmanaged DLL called ALVARBridge.dll. We need it, since ALVAR is in C++, and
Unity doesn't support it, but supports C# scripts instead. This allows us to call C++ functions in
C# scripts.

### The DLL
The following functions are used to detect markers in the image:
* `void alvar_init(int width, int height)` is used to initialize the detection of the markers.
* `void alvar_process(int* imageData, double* transMatrix)` detect the markers, and modify the
transformation matrix. This transformation matrix is then used in Unity to move the object on the
markers.

The ALVARBridge.dll is directly exported in the Assets\Plugins directory of the Unity application.

### Markers
Since this is inspired by DemoMarkerField, a demo application provided with the ALVAR library, the
markers used are the same as this demo. You can found it in the ALVAR.pdf.
<img src="https://raw.github.com/bara3r/AR-Unity/master/AR-Unity/Assets/Materials/markerfield.png"/>

AR-Unity
--------
This is the Unity project.

The video stream of the webcam is projected on a plane, and an object ressembling
a paper with markers is moving according to the detected markers. This object could be anything, as
long as it is associated with the Marker.cs script.

### Scene
There are four GameObjects in the scene:
* **Cube**: the object placed on the markers and moving according to their position and orientation.
* **Directional light**: used to light the plane.
* **Main Camera**: the Unity camera, filming the Plane object.
* **Plane**: the webcam's stream is projected on it.

### C# scripts
* **ALVARBridge.cs**: this is the bridge in Unity.
* **Background.cs**: this script uses ALVAR functions to detect markers in the webcam's stream.
* **Marker.cs**: moves an object associated with this script, according to the markers.
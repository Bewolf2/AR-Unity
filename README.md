AR-Unity
========

This is a Unity application, using the augmented reality library ALVAR.

It detects markers captured by the webcam, and applies an object on them.

ALVARBridge
-----------

This is the bridge generating the unmanaged DLL ALVARBridge.dll. We need it,
since ALVAR is in C++, and Unity doesn't support it, but supports C# scripts
instead.

The following functions are used to detect markers in the image:

* `void alvar_init(int width, int height)`
* `void alvar_process(int* imageData, double* transMatrix)`

ALVAR uses OpenCV to detect markers.

`alvar_init` is used to initialize the detection of the markers.

`alvar_process` detect the markers, and modify the transformation matrix.
This transformation matrix is then used in Unity to move the object on the markers.

AR-Unity
--------

This is the Unity project.

The video stream of the webcam is projected on a plane, and an object ressembling
a paper with markers is moving according to the detected markers.

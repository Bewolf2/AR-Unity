using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

public class background : MonoBehaviour {
	
	//The texture that holds the video captured by the webcam
	private WebCamTexture webCamTexture;

	//The selected webcam
	public int selectedCam = 0;
	    void Start()
	{
		//An integer that stores the number of connected webcams
	    int numOfCams = WebCamTexture.devices.Length;
		print (selectedCam);
		print (WebCamTexture.devices[selectedCam].name);
		//renderer.material.color = new Color (255, 255, 255);

		//Initialize the webCamTexture
		webCamTexture = new WebCamTexture();

        renderer.material.mainTexture = webCamTexture;
		webCamTexture.deviceName = WebCamTexture.devices[selectedCam].name;
		
		//Start streaming the images captured by the webcam into the texture
        webCamTexture.Play();
		
		// ALVAR
		ALVARDllBridge.alvar_init();
		
		int camID = ALVARDllBridge.alvar_add_camera("Calibrations/default_calib.xml", webCamTexture.width, webCamTexture.height);
		
		int markerRes = 5;
        double margin = 2;
		
		int detectorID = ALVARDllBridge.alvar_add_marker_detector(32.4f, markerRes, margin);
		
		// TODO: a initialiser!!
		IntPtr imageData;
		IntPtr interestedMarkerIDs;
		
		IntPtr idPtr = IntPtr.Zero;
        IntPtr posePtr = IntPtr.Zero;
		
		int numFoundMarkers;
        int numInterestedMarkers;
		double max_marker_error = 0.08;
        double max_track_error = 0.2;
		
		int prevMarkerNum = 0;
		int[] ids = null;
		double[] poseMats = null;
		
		Dictionary<int, Matrix> detectedMarkers = new Dictionary<int, Matrix>();
			
		ALVARDllBridge.alvar_detect_marker(detectorID, camID, 3, "RGB", "RGB", imageData, interestedMarkerIDs,
			numFoundMarkers, numInterestedMarkers, max_marker_error, max_track_error);
		
		// Fonction Process de Goblin
		if (numFoundMarkers <= 0)
                return;

        int id = 0;
        if (numInterestedMarkers > 0)
        {
            if (prevMarkerNum != numInterestedMarkers)
            {
				ids = new int[interestedMarkerNums];
                poseMats = new double[interestedMarkerNums * 16];
                idPtr = Marshal.AllocHGlobal(numInterestedMarkers * sizeof(int));
                posePtr = Marshal.AllocHGlobal(numInterestedMarkers * 16 * sizeof(double));
            }

            ALVARDllBridge.alvar_get_poses(detectorID, idPtr, posePtr);
			
			prevMarkerNum = numInterestedMarkers;
			
		    Marshal.Copy(idPtr, ids, 0, interestedMarkerNums);
            Marshal.Copy(posePtr, poseMats, 0, interestedMarkerNums * 16);

            for (int i = 0; i < interestedMarkerNums; i++)
            {
                id = ids[i];

                // If same marker ID exists, then we ignore the 2nd one
                if (detectedMarkers.ContainsKey(id))
                {
                    // do nothing
                }
                else
                {
                    int index = i * 16;
                    Matrix mat = new Matrix(
                        (float)poseMats[index], (float)poseMats[index + 1], (float)poseMats[index + 2], (float)poseMats[index + 3],
                        (float)poseMats[index + 4], (float)poseMats[index + 5], (float)poseMats[index + 6], (float)poseMats[index + 7],
                        (float)poseMats[index + 8], (float)poseMats[index + 9], (float)poseMats[index + 10], (float)poseMats[index + 11],
                        (float)poseMats[index + 12], (float)poseMats[index + 13], (float)poseMats[index + 14], (float)poseMats[index + 15]);
                    detectedMarkers.Add(id, mat);
                }
            }
        }

        /*if (multiMarkerIDs.Count == 0)
            return;

        double error = -1;

        ALVARDllBridge.alvar_get_multi_marker_poses(detectorID, cameraID, detectAdditional, 
            multiIdPtr, multiPosePtr, multiErrorPtr);

        Marshal.Copy(multiIdPtr, multiIDs, 0, multiMarkerIDs.Count);
        Marshal.Copy(multiPosePtr, multiPoseMats, 0, multiMarkerIDs.Count * 16);
        Marshal.Copy(multiErrorPtr, multiErrors, 0, multiMarkerIDs.Count);

        for (int i = 0; i < multiMarkerIDs.Count; i++)
        {
            id = multiIDs[i];
            error = multiErrors[i];

            if (error == -1)
                continue;

            int index = i * 16;
            Matrix mat = new Matrix(
                (float)multiPoseMats[index], (float)multiPoseMats[index + 1], (float)multiPoseMats[index + 2], (float)multiPoseMats[index + 3],
                (float)multiPoseMats[index + 4], (float)multiPoseMats[index + 5], (float)multiPoseMats[index + 6], (float)multiPoseMats[index + 7],
                (float)multiPoseMats[index + 8], (float)multiPoseMats[index + 9], (float)multiPoseMats[index + 10], (float)multiPoseMats[index + 11],
                (float)multiPoseMats[index + 12], (float)multiPoseMats[index + 13], (float)multiPoseMats[index + 14], (float)multiPoseMats[index + 15]);
            detectedMultiMarkers.Add(multiMarkerIDs[i], mat);
        }*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

// This class applies the stream of a webcam on the object associated with this script.
// It has a public transformation matrix, used by the Marker class.
public class background : MonoBehaviour {
	
	// The selected webcam
	public int selectedCam = 0;

	// The texture that holds the video captured by the webcam
	private WebCamTexture webCamTexture;
	
	// The pixels data in a raw format
	private Color32[] data;
    private int[] imageData;

    // This is the public transformation matrix, used in the Marker class
    public static double[] transMat = new double[16];

	void Start()
	{
        // Debug print
		print (selectedCam);
		print (WebCamTexture.devices[selectedCam].name);

		// Initialize the webCamTexture and apply the webcam stream on the object
		webCamTexture = new WebCamTexture();
        renderer.material.mainTexture = webCamTexture;
		webCamTexture.deviceName = WebCamTexture.devices[selectedCam].name;
		
		// Start streaming the images captured by the webcam into the texture
        webCamTexture.Play();

        // Initialisation of the bridge components
        ALVARBridge.alvar_init(webCamTexture.width, webCamTexture.height);
        data = new Color32[webCamTexture.width * webCamTexture.height];
        imageData = new int[data.Length * 3];
	}
	
	// Update is called once per frame
	void Update () {
        // The frame of the webcam is put in the data
        webCamTexture.GetPixels32(data);
        //int[] imageData = new int[data.Length * 3];

        // Convert the Color32[] in int*
        for (int i = 0; i < data.Length; ++i) {
            imageData[i * 3] = (int)data[i].b;
            imageData[i * 3 + 1] = (int)data[i].g;
            imageData[i * 3 + 2] = (int)data[i].r;
        }

        // The magic function which detects markers in the image and modify the transformation matrix
        ALVARBridge.alvar_process(imageData, transMat);

        // DEBUG
        Debug.Log("matrix={"
            + transMat[0].ToString("F2") + " " + transMat[1].ToString("F2") + " " + transMat[2].ToString("F2") + " " + transMat[3].ToString("F2") + " "
            + transMat[4].ToString("F2") + " " + transMat[5].ToString("F2") + " " + transMat[6].ToString("F2") + " " + transMat[7].ToString("F2") + " "
            + transMat[8].ToString("F2") + " " + transMat[9].ToString("F2") + " " + transMat[10].ToString("F2") + " " + transMat[11].ToString("F2") + " "
            + transMat[12].ToString("F2") + " " + transMat[13].ToString("F2") + " " + transMat[14].ToString("F2") + " " + transMat[15].ToString("F2") + "}");
	}
	
	void Close() {
		ALVARBridge.alvar_close();	
	}
}

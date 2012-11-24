using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

public class background : MonoBehaviour {
	
	//The texture that holds the video captured by the webcam
	private WebCamTexture webCamTexture;
	
	// The pixels data in raw format
	private Color32[] data;

	//The selected webcam
	public int selectedCam = 0;

    public float t;
	void Start()
	{
		//An integer that stores the number of connected webcams
	    //int numOfCams = WebCamTexture.devices.Length;
		print (selectedCam);
		print (WebCamTexture.devices[selectedCam].name);
		//renderer.material.color = new Color (255, 255, 255);

		//Initialize the webCamTexture
		webCamTexture = new WebCamTexture();

        renderer.material.mainTexture = webCamTexture;
		webCamTexture.deviceName = WebCamTexture.devices[selectedCam].name;
		
		//Start streaming the images captured by the webcam into the texture
        webCamTexture.Play();
        //data = new Color32[webCamTexture.width * webCamTexture.height];
		
        //webCamTexture.GetPixels32 (data);
        //// Need to convert Color32[] to char*
        //char[] imageData = new char[webCamTexture.width * webCamTexture.height * 3];

        //for (int i = 0; i < webCamTexture.width * webCamTexture.height; ++i) {
        //    imageData[i*3] = (char)data[i].r;
        //    imageData[i*3+1] = (char)data[i].g;
        //    imageData[i*3+2] = (char)data[i].b;
        //}

        //for (int i = 0; i < webCamTexture.width * webCamTexture.height; ++i)
        //{
        //    Debug.Log("r=" + imageData[i * 3] + " g=" + imageData[i * 3 + 1] +" b" + imageData[i * 3 + 2]);
        //}


        /*IntPtr c = Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0);
        ALVARBridge.alvar_init(c, webCamTexture.width, webCamTexture.height);*/
        /*IntPtr ptr = new IntPtr();
        Marshal.StructureToPtr(imageData, ptr, true);*/
        ALVARBridge.alvar_init(webCamTexture.width, webCamTexture.height);
        //ALVARBridge.alvar_init(imageData, webCamTexture.width, webCamTexture.height);


        t = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log("data=" + data.Length);
        //Debug.Log("cam=" + webCamTexture.width * webCamTexture.height);
        //Debug.Log("width=" + webCamTexture.width + " height=" + webCamTexture.height);


        /*Debug.Log("l=" + imageData.Length);
        Debug.Log("0=" + imageData[0] + " " + imageData[1] + " "+ imageData[2]);
        Debug.Log("$=" + imageData[imageData.Length - 3] + " " + imageData[imageData.Length - 2] + " " + imageData[imageData.Length - 1]);*/


        //for (int i = 0; i < imageData.Length; ++i)
        //{
        //    //Debug.Log(imageData[i]);
        //    Debug.Log("r=" + imageData[i * 3] + " g=" + imageData[i * 3 + 1] + " b" + imageData[i * 3 + 2]);
        //}
		
        double[] transMat = new double[16];
        ///*IntPtr c = Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0);
        //ALVARBridge.alvar_process(c, transMat);*/
        if (Time.time > t + 2) {
            t = Time.time;

            data = new Color32[webCamTexture.width * webCamTexture.height];
            webCamTexture.GetPixels32 (data);
            int[] imageData = new int[data.Length * 3];

            for (int i = 0; i < data.Length; ++i) {
                imageData[i*3] = (int)data[i].b;
                imageData[i*3+1] = (int)data[i].g;
                imageData[i*3+2] = (int)data[i].r;
            }
            

            ALVARBridge.alvar_process(imageData, transMat);

            Debug.Log("matrix={"
            + transMat[0].ToString("F2") + " " + transMat[1].ToString("F2") + " " + transMat[2].ToString("F2") + " " + transMat[3].ToString("F2") + " "
            + transMat[4].ToString("F2") + " " + transMat[5].ToString("F2") + " " + transMat[6].ToString("F2") + " " + transMat[7].ToString("F2") + " "
            + transMat[8].ToString("F2") + " " + transMat[9].ToString("F2") + " " + transMat[10].ToString("F2") + " " + transMat[11].ToString("F2") + " "
            + transMat[12].ToString("F2") + " " + transMat[13].ToString("F2") + " " + transMat[14].ToString("F2") + " " + transMat[15].ToString("F2") + "}");
        }
		
        //int nb = ALVARBridge.alvar_number_of_detected_markers();

        //if (nb > 0)
        //{
        //    /*Debug.Log(nb + " markers found with matrix={"
        //        + transMat[0].ToString("F2") + " " + transMat[1].ToString("F2") + " " + transMat[2].ToString("F2") + " " + transMat[3].ToString("F2") + " "
        //        + transMat[4].ToString("F2") + " " + transMat[5].ToString("F2") + " " + transMat[6].ToString("F2") + " " + transMat[7].ToString("F2") + " "
        //        + transMat[8].ToString("F2") + " " + transMat[9].ToString("F2") + " " + transMat[10].ToString("F2") + " " + transMat[11].ToString("F2") + " "
        //        + transMat[12].ToString("F2") + " " + transMat[13].ToString("F2") + " " + transMat[14].ToString("F2") + " " + transMat[15].ToString("F2") + "}");*/
        //}
        //else
        //{
        //    //Debug.Log("No markers found");
        //}
	}
	
	void Close() {
		//ALVARBridge.alvar_close();	
	}
}

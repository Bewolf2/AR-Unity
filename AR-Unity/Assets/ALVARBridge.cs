using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

// This is the bridge of the ALVAR library in Unity.
// It imports the functions of the DLL in this special class.
public class ALVARBridge {

    //[DllImport("ALVARBridge", EntryPoint = "alvar_init", CallingConvention = CallingConvention.Cdecl)]
    [DllImport("ALVARBridge")]
    public static extern void alvar_init(
		int width, 
        int height);

    [DllImport("ALVARBridge")]
    public extern static void alvar_process(
        int[] imageData,
        double[] transMatrix);

    // This function seems buggy.
    [DllImport("ALVARBridge")]
    public extern static int alvar_number_of_detected_markers(int[] imageData);

    [DllImport("ALVARBridge", EntryPoint = "alvar_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_close();
}
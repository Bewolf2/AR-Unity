using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class ALVARBridge {

    [DllImport("ALVARBridge", EntryPoint = "alvar_init", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_init(
		int width, 
        int height);
	
	[DllImport("ALVARBridge.dll")]
    public extern static void alvar_process(
        int[] imageData,
        double[] transMatrix);

    /*[DllImport("ALVARBridge", EntryPoint = "alvar_number_of_detected_markers", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_number_of_detected_markers();*/

    [DllImport("ALVARBridge", EntryPoint = "alvar_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_close();
}
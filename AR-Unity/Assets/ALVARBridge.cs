using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ALVARBridge {

    [DllImport("ALVARBridge", EntryPoint = "alvar_init", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_init(
        /*IntPtr imageData,*/
        //[MarshalAs(UnmanagedType.LPArray)] char[] imageData,
		//char[] imageData,
		/*int width, 
        int height*/);
	
	[DllImport("ALVARBridge", EntryPoint = "alvar_process", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_process(
        /*IntPtr imageData,*/
        //[MarshalAs(UnmanagedType.LPArray)] char[] imageData,
		//char[] imageData,
		[Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] double[] transMatrix);

    [DllImport("ALVARBridge", EntryPoint = "alvar_number_of_detected_markers", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_number_of_detected_markers();

    [DllImport("ALVARBridge", EntryPoint = "alvar_close", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_close();
}
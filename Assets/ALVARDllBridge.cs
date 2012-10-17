using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class ALVARDllBridge {
    [DllImport("ALVARWrapper", EntryPoint = "alvar_init", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_init();

    [DllImport("ALVARWrapper", EntryPoint = "alvar_add_camera", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_add_camera(
        string calibFile, 
        int width, 
        int height);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_add_fern_estimator", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_add_fern_estimator(
        string calibFile,
        int width,
        int height);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_get_camera_projection", CallingConvention = CallingConvention.Cdecl)]
    private static extern void alvar_get_camera_projection(
        string calibFile, 
        int width, 
        int height,
        float farClip, 
        float nearClip,
        [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] double[] projMatrix);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_get_camera_params", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_get_camera_params(
        int camID,
        [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] double[] projMatrix,
        ref double fovX, 
        ref double fovY, 
        float farClip, 
        float nearClip);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_add_marker_detector", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_add_marker_detector(
        double markerSize, 
        int markerRes,
        double margin);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_train_feature", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_train_feature(
        string imageFilename,
        string classifierFilename);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_add_feature_detector", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_add_feature_detector(
        string classifierFilename);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_set_marker_size", CallingConvention = CallingConvention.Cdecl)]
    public static extern int alvar_set_marker_size(
        int detectorID, 
        int markerID, 
        double markerSize);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_add_multi_marker", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_add_multi_marker(String filename);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_detect_feature", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool alvar_detect_feature(
        int camID,
        int numChannels,
        string colorModel,
        string channelSeq,
        IntPtr imageData,
        double minInlierRatio,
        int minMappedPoints,
        ref double inlierRatio,
        ref int mappedPoints);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_detect_marker", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_detect_marker(
        int detectorID, 
        int camID,
        int numChannels, 
        string colorModel,
        string channelSeq, 
        IntPtr imageData, 
        [In, Out] IntPtr interestedMarkerIDs,
        ref int numFoundMarkers, 
        ref int numInterestedMarkers, 
        double maxMarkerError, 
        double maxTrackError);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_get_poses", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_get_poses(
        int detectorID,
        [Out] IntPtr ids,
        [Out] IntPtr poseMatrices);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_get_feature_pose", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_get_feature_pose(
        [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] double[] poseMatrices);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_get_multi_marker_poses", CallingConvention = CallingConvention.Cdecl)]
    public static extern void alvar_get_multi_marker_poses(
        int detectorID,
        int camID,
        bool detectAdditional,
        [Out] IntPtr ids,
        [Out] IntPtr poseMatrices,
        [Out] IntPtr errors);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_calibrate_camera", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool alvar_calibrate_camera(
        int camID, 
        int numChannels, 
        string colorModel,
        string channelSeq, 
        IntPtr imageData, 
        double etalon_square_size, 
        int etalon_rows,
        int etalon_columns);

    [DllImport("ALVARWrapper", EntryPoint = "alvar_finalize_calibration", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool alvar_finalize_calibration(
        int camID, 
        string calibrationFilename);

    #region Static Helpers

    /*public static Matrix GetCameraProjection(string calibFilename, int width, int height, float nearClipPlane,
        float farClipPlane)
    {
        double[] projMat = new double[16];

        alvar_get_camera_projection(calibFilename, width, height, farClipPlane, nearClipPlane, projMat);

        return new Matrix(
            (float)projMat[0], (float)projMat[1], (float)projMat[2], (float)projMat[3],
            (float)projMat[4], (float)projMat[5], (float)projMat[6], (float)projMat[7],
            (float)projMat[8], (float)projMat[9], (float)projMat[10], (float)projMat[11],
            (float)projMat[12], (float)projMat[13], (float)projMat[14], (float)projMat[15]);
    }*/

    #endregion*/
}
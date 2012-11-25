//ALVAR includes
#include "CaptureFactory.h" // Video capturing
#include "MarkerDetector.h" // Marker detector
#include "MultiMarker.h"    // Multimarker system (marker field)
#include "TrackerStat.h"    // Visual based tracking system
#include "Marker.h"
#include "highgui.h"

int w = 0; // Video capture width (x)
int h = 0; // video capture height (y)

alvar::Camera camera; // ALVAR camera (do not confuse to the OSG)
alvar::MarkerDetector<alvar::MarkerData> markerDetector; // Marker detector
alvar::TrackerStat trackerStat; // Visual tracker
alvar::MultiMarker *multiMarker;// MultiMarker
std::vector<int> markerIdVector;// vector that contains marker field marker ids

// Size of the markers in the marker field
// The size is an "abstract value" for library, but using normal, logical values (mm, cm, m, inch) will help 
// understanding the model scaling and positioning in human point of view.
#define CORNER_MARKER_SIZE	4 // as in centimeters
#define CENTRE_MARKER_SIZE  8 // as in centimeters
#define MARKER_COUNT		5 // marker count in the field

// Global variables
// The image
IplImage *image;
// A temporary array
char *tmp;

extern "C"
{
	__declspec(dllexport) void alvar_init(int width, int height)
	{
		w = width;
		h = height;

		// Calibration. See manual and ALVAR internal samples how to calibrate your camera
		// Calibration will make the marker detecting and marker pose calculation more accurate.
		if (! camera.SetCalib("Calibrations/default_calib.xml", w, h)) {
			camera.SetRes(w, h);
		}
		
		// Set projection matrix as ALVAR recommends (based on the camera calibration)
		double p[16];
		camera.GetOpenglProjectionMatrix(p, w, h);

		//Initialize the multimarker system
		for(int i = 0; i < MARKER_COUNT; ++i)
			markerIdVector.push_back(i);

		// We make the initialization for MultiMarkerBundle using a fixed marker field (can be printed from MultiMarker.ppt)
		markerDetector.SetMarkerSize(CORNER_MARKER_SIZE);
		markerDetector.SetMarkerSizeForId(0, CENTRE_MARKER_SIZE);
		
		multiMarker = new alvar::MultiMarker(markerIdVector);
		
		alvar::Pose pose;
		pose.Reset();

		// Add the 5 markers
		multiMarker->PointCloudAdd(0, CENTRE_MARKER_SIZE, pose);
		
		pose.SetTranslation(-10, 6, 0);
		multiMarker->PointCloudAdd(1, CORNER_MARKER_SIZE, pose);
		
		pose.SetTranslation(10, 6, 0);
		multiMarker->PointCloudAdd(2, CORNER_MARKER_SIZE, pose);
		
		pose.SetTranslation(-10, -6, 0);
		multiMarker->PointCloudAdd(3, CORNER_MARKER_SIZE, pose);
		
		pose.SetTranslation(+10, -6, 0);
		multiMarker->PointCloudAdd(4, CORNER_MARKER_SIZE, pose);

		trackerStat.Reset();
	}

	__declspec(dllexport) void alvar_process(int* imageData, double* transMatrix)
	{
		alvar::Pose pose;

		// Initialisation of the image
		image = new IplImage();
		image->nSize = sizeof(IplImage);
		image->ID = 0;
		image->nChannels = 3;
		image->alphaChannel = 0;
		image->depth = IPL_DEPTH_8U;

		memcpy(&image->colorModel, "RGB", sizeof(char) * 4);
		memcpy(&image->channelSeq, "RGB", sizeof(char) * 4);
		image->dataOrder = 0;

		image->origin = 0;
		image->align = 4;
		image->width = w;
		image->height = h;

		image->roi = NULL;
		image->maskROI = NULL;
		image->imageId = NULL;
		image->tileInfo = NULL;
		image->widthStep = w * 3;
		image->imageSize = h * image->widthStep;

		image->imageDataOrigin = NULL;

		int n = w * h * 3;
		tmp = new char[n];

		// We put the image from Unity in an IplImage
		// The Unity image is in RGB, and the OpenCV is BGR.
		// Moreover, the Unity one begins in the corner lower-left
		// while the OpenCV one begins in the upper-left corner.
		for (int i = 0; i < (w*3); ++i)
			for (int j = 0; j < h; ++j)
				tmp[i + j * (w*3)] = (char)imageData[i + (h - j - 1) * (w*3)];

		// We put the data in the image
		image->imageData = tmp;

		// Detect all the markers from the frame
		markerDetector.Detect(image, &camera, false, false);
		trackerStat.Track(image);

		// Detect the markers
		if (markerDetector.Detect(image, &camera, false, false)) {
			// if ok, we have field in sight
			// Update the data
			multiMarker->Update(markerDetector.markers, &camera, pose);

			// get the field's matrix
			pose.GetMatrixGL(transMatrix);
			trackerStat.Reset();
		}
	
		// Clean
		delete tmp;
		delete image;
	}

	// This function returns the number of detected markers.
	// It uses just OpenCV capture, but should be removed soon.
	__declspec(dllexport) int alvar_number_of_detected_markers(int* imageData)
	{
		alvar::Pose pose;

		image->imageData = tmp;

		// Detect all the markers from the frame
		markerDetector.Detect(image, &camera, false, false);
		trackerStat.Track(image);

		// Detect the markers
		if (markerDetector.Detect(image, &camera, false, false)) {
			// if ok, we have field in sight
			// Update the data
			multiMarker->Update(markerDetector.markers, &camera, pose);

			trackerStat.Reset();
		}
	
		// Clean
		delete tmp;
		delete image;

		return markerDetector.markers->size();
	}

	__declspec(dllexport) void alvar_close()
	{
		delete image;
	}
}
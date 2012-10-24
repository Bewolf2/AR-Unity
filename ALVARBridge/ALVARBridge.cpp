//ALVAR includes
#include "CaptureFactory.h" // Video capturing
#include "MarkerDetector.h" // Marker detector
#include "MultiMarker.h"    // Multimarker system (marker field)
#include "TrackerStat.h"    // Visual based tracking system
#include "Marker.h"
#include "highgui.h"

int videoXRes=0; // Video capture width (x)
int videoYRes=0; // video capture height (y)
//float video_X_FOV; // Video horizontal field of view
//float video_Y_FOV; // Video vertical field of view
//alvar::Capture*	capture = NULL; // ALVAR Capture

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

// Global variable
//alvar::CaptureFactory *factory;
IplImage *image;
//IplImage *tempImg;
// Comment this in a near future
CvCapture* capture;

extern "C"
{
	__declspec(dllexport) void alvar_init(/*char imageData[], int width, int height*/)
	{
		/*image->nSize = sizeof(IplImage);
		image->ID = 0;
		image->nChannels = 3;
		image->alphaChannel = 0;
		image->depth = IPL_DEPTH_8U;

		memcpy(&image->colorModel, "RGB", sizeof(char) * 4);
		memcpy(&image->channelSeq, "RGB", sizeof(char) * 4);
		image->dataOrder = 0;

		image->origin = 0;
		image->align = 4;
		image->width = width;
		image->height = height;

		image->roi = NULL;
		image->maskROI = NULL;
		image->imageId = NULL;
		image->tileInfo = NULL;
		image->widthStep = width * 3;
		image->imageSize = height * image->widthStep;

		image->imageData = imageData;
		image->imageDataOrigin = NULL;*/

		capture = cvCaptureFromCAM(CV_CAP_ANY);

		// Capture is central feature, so if we fail, we get out of here.
		if (capture) {
		//if (image) {
			// Let's capture one frame to get video resolution
			image = cvQueryFrame(capture);

			videoXRes = image->width;
			videoYRes = image->height;
			/*videoXRes = width;
			videoYRes = height;*/

			// Calibration. See manual and ALVAR internal samples how to calibrate your camera
			// Calibration will make the marker detecting and marker pose calculation more accurate.
			if (! camera.SetCalib("Calibrations/default_calib.xml", videoXRes, videoYRes)) {
				camera.SetRes(videoXRes, videoYRes);
			}

			// Get the video fov for the tracking system
			/*video_X_FOV = camera.GetFovX();
			video_Y_FOV = camera.GetFovY();*/
		
			// Set projection matrix as ALVAR recommends (based on the camera calibration)
			double p[16];
			camera.GetOpenglProjectionMatrix(p,videoXRes,videoYRes);

			//Initialize the multimarker system
			for(int i = 0; i < MARKER_COUNT; ++i)
				markerIdVector.push_back(i);

			// We make the initialization for MultiMarkerBundle using a fixed marker field (can be printed from MultiMarker.ppt)
			markerDetector.SetMarkerSize(CORNER_MARKER_SIZE);
			markerDetector.SetMarkerSizeForId(0, CENTRE_MARKER_SIZE);
		
			multiMarker = new alvar::MultiMarker(markerIdVector);
		
			alvar::Pose pose;
			pose.Reset();
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
	}

	__declspec(dllexport) void alvar_process(/*char imageData[], */double* transMatrix)
	{
		alvar::Pose pose;
		// Capture the image
		image = cvQueryFrame(capture);
		//image->imageData = imageData;

		// Check if we need to change image origin and is so, flip the image.
		bool flip_image = (image->origin?true:false);
		if (flip_image) {
			cvFlip(image);
			image->origin = !image->origin;
		}

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
	
		// In case we flipped the image, it's time to flip it back 
		if (flip_image) {
			cvFlip(image);
			image->origin = !image->origin;
		}
	}

	// This function returns the number of detected markers.
	// It uses just OpenCV capture, but should be removed soon.
	__declspec(dllexport) int alvar_number_of_detected_markers()
	{
		alvar::Pose pose;
		// Capture the image
		image = cvQueryFrame(capture);

		// Check if we need to change image origin and is so, flip the image.
		bool flip_image = (image->origin?true:false);
		if (flip_image) {
			cvFlip(image);
			image->origin = !image->origin;
		}

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
	
		// In case we flipped the image, it's time to flip it back 
		if (flip_image) {
			cvFlip(image);
			image->origin = !image->origin;
		}
		return markerDetector.markers->size();
	}

	__declspec(dllexport) void alvar_close()
	{
		// Time to close the system
		if(capture){
			cvReleaseCapture(&capture);
			delete capture;
		}
		//delete image;
	}
}
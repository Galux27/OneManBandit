using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_CameraView  : PhoneTab {
	public Camera secondCamera;
	public int counter = 0;
	public RawImage background;
	public Button nextCam, prevCam,rotateAnti,rotateClock;
	public Text noInput;
	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		noInput.gameObject.SetActive(false);
		rotateAnti.gameObject.SetActive(false);
		rotateClock.gameObject.SetActive (false);
		nextCam.gameObject.SetActive(false);
		prevCam.gameObject.SetActive (false);
		background.enabled = false;
		secondCamera.gameObject.SetActive (false);
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		if (CCTVController.me.camerasInWorld.Length == 0) {
			noInput.gameObject.SetActive (true);
		}
		secondCamera.gameObject.SetActive (true);

		rotateAnti.gameObject.SetActive(true);
		rotateClock.gameObject.SetActive (true);
		nextCam.gameObject.SetActive(true);
		prevCam.gameObject.SetActive (true);
		background.enabled=true;
		active = true;
	}

	public override void onUpdate()
	{
		if (ComputerTabCamera.loopedCameras == false) {
			//do any shit that needs to be done every frame
			if (CCTVController.me.camerasInWorld.Length == 0) {
				noInput.gameObject.SetActive (true);
				background.enabled = false;
			} else {//will need to add some condition to check if cameras have been disabled by the player
				noInput.gameObject.SetActive (false);
				background.enabled = true;

				setCameraPos ();
			}
		} else {
			noInput.gameObject.SetActive (true);
			background.enabled = false;
		}

	}

	void setCameraPos()
	{
		CCTVCamera cameraToSee = CCTVController.me.camerasInWorld [counter];
		secondCamera.transform.position = new Vector3 (cameraToSee.transform.position.x, cameraToSee.transform.position.y, -10)+ (cameraToSee.gameObject.transform.up.normalized*2.5f);
	}

	public void rotateClockwise()
	{
		CCTVCamera cameraToSee = CCTVController.me.camerasInWorld [counter];
		cameraToSee.rotateCounterClockwise ();
	}

	public void rotateAntiClockwise()
	{
		CCTVCamera cameraToSee = CCTVController.me.camerasInWorld [counter];
		cameraToSee.rotateClockwise();
	}

	public void increaseCounter()
	{
		if (counter < CCTVController.me.camerasInWorld.Length-1) {
			counter++;
		}
	}

	public void decreaseCounter()
	{
		if (counter > 0) {
			counter--;
		}
	}

}

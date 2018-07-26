using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComputerTabCamera : ComputerTab {
	public Camera secondCamera;

	public GameObject selectedCamera;
	public CameraViewPrefab display;
	public Dropdown cameraSelect;
	public RawImage cameraView;
	public RenderTexture cameraTex;
	public bool drawImage = true;
	public static bool loopedCameras = false;
	// Use this for initialization
	void Start () {
		
	
		cameraSelect.AddOptions (getDropDownText ());
	}
	
	// Update is called once per frame
	void Update () {
		
		if (drawImage == true) {
			selectedCamera =  CCTVController.me.camerasInWorld[cameraSelect.value].gameObject;
			secondCamera.transform.position = new Vector3 (selectedCamera.transform.position.x, selectedCamera.transform.position.y, -10)+ (selectedCamera.gameObject.transform.up.normalized*2.5f);
			setTextForCamera ();
			cameraView.color = Color.white;
			cameraView.texture = cameraTex;
		} else {
			cameraView.color = Color.black;
			setTextForCamera ();

		}
	}

	public List<string> getDropDownText(){
		List<string> retVal = new List<string> ();
		foreach (CCTVCamera g in CCTVController.me.camerasInWorld) {
			retVal.Add (g.gameObject.name);
		}
		return retVal;
	}

	void setTextForCamera()
	{
		//if (display.cameraName.text != "Cam " + cameraSelect.value) {
			display.cameraName.text = "Cam " + cameraSelect.value;
			RoomScript r = LevelController.me.getRoomObjectIsIn (selectedCamera);
			if (r == null) {
				display.cameraRoom.text = "Outdoors";
			} else {
				display.cameraRoom.text = r.roomName;
			}

			if (drawImage == false) {
				display.cameraActive.text = "Deactivated";
			} else {
				display.cameraActive.text = "Active";

			}
		//}
	}

	public void rotateClockwise()
	{
		CCTVCamera cameraToSee = CCTVController.me.camerasInWorld [cameraSelect.value];
		cameraToSee.rotateClockwise ();
	}

	public void rotateAntiClockwise()
	{
		CCTVCamera cameraToSee = CCTVController.me.camerasInWorld [cameraSelect.value];
		cameraToSee.rotateCounterClockwise ();
	}

	public override void downloadData ()
	{
		//PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("CCTVView"));
		if (PhoneTab_DownloadingHack.me.gettingHack == false && ComputerTabCamera.loopedCameras==false) {
			PhoneTab_DownloadingHack.me.setHack (CommonObjectsStore.player.transform.position, "CCTVView", 20000);
		}
	}

	public void disableButtonPress(){
		if (drawImage==false) {
			enableCameras ();
		} else {
			disableCameras ();
		}
	}

	public void enableCameras()
	{
		foreach (CCTVCamera c in CCTVController.me.camerasInWorld) {
			c.cameraActive = true;
		}
		drawImage = true;
		ComputerTabCamera.loopedCameras = false;
	}

	public void disableCameras()
	{
		foreach (CCTVCamera c in CCTVController.me.camerasInWorld) {
			c.cameraActive = false;
		}
		drawImage = false;
		ComputerTabCamera.loopedCameras = true;
	}

}

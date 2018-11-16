using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that controls the players input & some movement
/// </summary>
public class PlayerInputController : MonoBehaviour {
	
	public static PlayerInputController me;
	PersonMovementController pmc;
	public PersonWeaponController pwc;
	void Awake()
	{
		me = this;
		pmc = this.gameObject.GetComponent<PersonMovementController> ();
		pwc = this.gameObject.GetComponent<PersonWeaponController> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//movePerson ();
		pmc.physicsMoveTest();
	}

	void Update()
	{
		//displayNearestNode ();
		pauseMenuDetect ();

		if (PauseMenu.me.pauseGUIParent.activeInHierarchy == true) {
			return;
		}

		if (MapControlScript.me.displayMap == false) {
			if (PlayerCarController.inCar == false) {
				pmc.moveDirSet ();
				rotateToMouse ();
				AimDownSight ();
				fireWeapon ();
				lookAheadControl ();
				dropWeapon ();
			}
			openInventory ();
			openPhoneShortcut ();
			debugStuff ();
		}
	}

	/// <summary>
	/// Just a method for testing out things in the editor, to be removed in final build.
	/// </summary>
	void debugStuff()
	{
		if (Application.isEditor) {
			Vector3 headingToPoint = (this.transform.position+new Vector3(0,1,0)) - (this.transform.position-new Vector3(0,1,0)) ;
			float distance = headingToPoint.magnitude;

			float dotProduct = Vector3.Dot (headingToPoint, this.transform.position);
	
			if (Input.GetKeyDown (KeyCode.O)) {
				
				NPCController[] npcs = FindObjectsOfType<NPCController> ();
				foreach (NPCController npc in npcs) {
					if (npc.npcB.myType == AIType.guard) {
						npc.memory.beenAttacked = true;
						npc.memory.seenCorpse = true;
						npc.memory.seenArmedSuspect = true;
						npc.memory.seenSuspect = true;
						npc.memory.seenHostage = true;
						npc.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						PoliceController.me.swatTimer = 1;
						PoliceController.me.policeBackup = 1;
						break;
					}
				}
			}
		}
	}


	/// <summary>
	/// Player is rotated here rather than in its person movement script due to the camera perspective warping the position of the mouse.
	/// </summary>
	void rotateToMouse()
	{
		if (CommonObjectsStore.me.mainCam.orthographic == true) {
			if (PlayerActionUI.me.lastObjectSelected == null) {
				Vector3 position = CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);
				pmc.rotateToFacePosition (position);
			} else {
				Vector3 position = PlayerActionUI.me.lastObjectSelected.transform.position;
				pmc.rotateToFacePosition (position);
			}
		} else {

			if (PlayerActionUI.me.lastObjectSelected == null) {
				pmc.rotateToFacePosition (getMousePositionWithPerspective(Input.mousePosition,this.transform.position.z));
			} else {
				pmc.rotateToFacePosition (getMousePositionWithPerspective(Input.mousePosition,PlayerActionUI.me.lastObjectSelected.transform.position.z));
			}
		}
	}

	Vector3 getMousePositionWithPerspective(Vector3 screenPos, float z)
	{
		Ray ray = Camera.main.ScreenPointToRay (screenPos);
		Plane xy = new Plane (Vector3.forward, new Vector3 (0, 0, z));
		float distance;
		xy.Raycast (ray, out distance);
		return ray.GetPoint (distance);
	}

	void movePerson()
	{
	//	if (Input.GetKey (KeyCode.W)) {
			pmc.moveUp ();
		//}

		//if (Input.GetKey (KeyCode.A)) {
			pmc.moveLeft();
		//}

		//if (Input.GetKey (KeyCode.S)) {
			pmc.moveDown ();
		//}
	//
		//if (Input.GetKey (KeyCode.D)) {
			pmc.moveRight ();
		//}



	}

	void AimDownSight()
	{
		if (pwc.currentWeapon == null || pwc.currentWeapon.melee == true) {
			if (Input.GetMouseButton (1)) {
				pwc.block ();	
				pwc.aimDownSight = false;

			} else {
				pwc.stopBlocking ();

			}
		} else {
			if (Input.GetMouseButton (1)) {
				pwc.aimDownSight = true;	
			} else {
				pwc.aimDownSight = false;
			}
		}
	}

	void fireWeapon()
	{
		if (Input.GetMouseButton (0) && shouldWeBeAbleToClick()) {
			pwc.fireWeapon ();
		}
	}

	void dropWeapon()
	{
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			pwc.dropWeapon ();
		}

	
	}

	void openInventory()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (InventorySwitch.me.switchParent.activeInHierarchy == true) {
				InventorySwitch.me.disable ();
			} else {
				InventorySwitch.me.enable ();
			}
		
		}
	}

	void lookAheadControl()
	{
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			CameraController.me.doWeFollowPlayer = false;
		}

		if (Input.GetKey (KeyCode.LeftShift)) {
			CameraController.me.moveCameraInMouseDir ();
		}

		if (Input.GetKeyUp (KeyCode.LeftShift)) {
			CameraController.me.doWeFollowPlayer = true;
		}
	}

	void openPhoneShortcut()
	{
		if (Input.GetKeyDown (KeyCode.T)) {
			PhoneController.me.openPhone ();
		}

		if (PhoneController.me.phoneOpen() == true) {
			if (Input.GetKeyDown (KeyCode.G)) {
				PhoneController.me.previousTab ();
			}

			if (Input.GetKeyDown (KeyCode.H)) {
				PhoneController.me.nextTab ();
			}

		}
	}

	/// <summary>
	/// Checks for UI elements that are active that should stop the player attacking e.g. shops, conversations,keypads etc... to stop the player attacking accidently
	/// </summary>
	/// <returns><c>true</c>, if we be able to click was shoulded, <c>false</c> otherwise.</returns>
	public bool shouldWeBeAbleToClick()
	{
		if (Keypad.me.isKeypadInUse()==true|| PlayerActionUI.me.isMenuOpen () == true || Inventory.playerInventory.inventoryGUI.activeInHierarchy==true|| ExamineItem.me.isExaminingItem()==true || PhoneController.me.phoneOpen() || ComputerUIControl.me.computerBeingUsed==true || InventorySwitch.me.switchParent.activeInHierarchy==true || CraftingUIParent.me.craftObj.activeInHierarchy==true || ConversationUI.me.convParent.activeInHierarchy==true || ShopUI.me.shopParent.activeInHierarchy==true) {
			return false;
		} else {
			return true;
		}
	}

	void pauseMenuDetect()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			PauseMenu.me.openPauseMenu ();
		}
	}


}
